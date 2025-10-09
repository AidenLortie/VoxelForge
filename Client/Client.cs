using System.Net;
using System.Net.Sockets;
using VoxelForge.Shared.Content.Blocks;
using VoxelForge.Shared.Networking;
using VoxelForge.Shared.Networking.NetworkBridge;
using VoxelForge.Shared.Networking.Packets;
using VoxelForge.Shared.Registry;
using VoxelForge.Shared.World;

namespace VoxelForge.Client;

// Game client - handles connection to server, receives world data, sends player actions
public class Client
{
    private readonly INetworkBridge _bridge;
    private readonly World _world;
    private Action<Chunk>? _chunkUpdateHandler;
    private int _chunksLoaded = 0;
    private int _chunksRequested = 0;
    private bool _initialLoadComplete = false;
    
    public World World => _world;
    public bool IsInitialLoadComplete => _initialLoadComplete;

    public Client(INetworkBridge bridge)
    {
        _bridge = bridge;
        
        // Initialize default blocks
        DefaultBlocks.Initialize();
        
        // Create a local world to match server (16x16 chunks)
        _world = new World(16, 1, 16);
    }
    
    /// <summary>
    /// Sets a handler that will be called when a chunk is received or updated.
    /// </summary>
    /// <param name="handler">The handler to call with the chunk</param>
    public void SetChunkUpdateHandler(Action<Chunk> handler)
    {
        _chunkUpdateHandler = handler;
    }
    
    /// <summary>
    /// Requests chunks around a position with the specified view distance.
    /// </summary>
    // Request chunks in a grid around center position
    public void RequestChunksAround(int centerX, int centerZ, int viewDistance = 4)
    {
        _chunksRequested = 0;
        for (int x = centerX - viewDistance; x <= centerX + viewDistance; x++)
        {
            for (int z = centerZ - viewDistance; z <= centerZ + viewDistance; z++)
            {
                // Only request chunks within world bounds
                if (x >= 0 && x < 16 && z >= 0 && z < 16)
                {
                    RequestChunk(x, z);
                    _chunksRequested++;
                }
            }
        }
        Console.WriteLine($"Requested {_chunksRequested} chunks");
    }

    // Main entry point - shows menu then starts client in selected mode
    public static void Main(string[] args)
    {
        // Check for command line override
        bool forceRendering = args.Contains("--rendering");
        bool forceConsole = args.Contains("--console");
        
        if (forceRendering)
        {
            Console.WriteLine("Starting in single player mode (command line override)...");
            RunSinglePlayer();
        }
        else if (forceConsole)
        {
            Console.WriteLine("Starting in multiplayer mode (command line override)...");
            RunMultiplayer();
        }
        else
        {
            // Show menu and let user choose
            using var menu = new UI.MenuWindow();
            menu.Run();
            
            if (menu.ShouldStartSinglePlayer)
            {
                RunSinglePlayer();
            }
            else if (menu.ShouldStartMultiplayer)
            {
                RunMultiplayer();
            }
            else
            {
                Console.WriteLine("Exiting...");
            }
        }
    }
    
    // Run in single player mode with local server
    private static void RunSinglePlayer()
    {
        // Create local bridges for client and server
        var clientBridge = new NetworkBridgeLocal();
        var serverBridge = new NetworkBridgeLocal();
        clientBridge.ConnectTo(serverBridge);
        
        // Start local server in background
        var server = new VoxelForge.Server.Server(serverBridge);
        Task.Run(async () => await server.RunAsync());
        
        Console.WriteLine("Local server started");

        var client = new Client(clientBridge);

        // Start listening for incoming packets
        client.StartListening();

        // Request chunks around spawn (8, 8 is center of 16x16 world)
        client.RequestChunksAround(8, 8, 4); // Request 4 chunks in each direction
        
        // Create and run the rendering window
        using var window = new Rendering.GameWindow(client);
        
        // Set up chunk update handler
        client.SetChunkUpdateHandler(chunk =>
        {
            window.ChunkRenderer?.UpdateChunk(chunk);
        });
        
        window.Run();
        
        Console.WriteLine("Window closed");
    }
    
    // Run in multiplayer mode connecting to network server
    private static void RunMultiplayer()
    {
        // Connect to server
        var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(IPAddress.Loopback, 25565);

        var stream = new NetworkStream(clientSocket);
        var bridgeNet = new NetworkBridgeNet(stream, PacketRegistry.Factories);

        var client = new Client(bridgeNet);

        // Start listening for incoming packets
        client.StartListening();

        // Request chunks around spawn
        client.RequestChunksAround(8, 8, 2); // Request 2 chunks in each direction for console mode
        
        // Main loop
        while (true)
        {
            client.Poll();
            client.SendPacket(new CheckPacket());
            Task.Delay(100).Wait();
        }
    }

    // Send packet to server
    public void SendPacket(Packet packet)
    {
        _bridge.Send(packet);
    }
    
    // Poll network bridge to process incoming packets - call regularly in game loop
    public void Poll()
    {
        _bridge.Poll();
    }

    // Register packet handlers - call once after creating client
    public void StartListening()
    {
        _bridge.RegisterHandler<CheckPacket>(_ => { Console.WriteLine("Received check packet from server."); });
        
        _bridge.RegisterHandler<BlockStateRegistryPacket>(packet =>
        {
            Console.WriteLine($"Received BlockState registry ({packet.StateIdToString.Count} states)");
            BlockStateRegistry.ImportMappings(packet.StateIdToString);
            Console.WriteLine("BlockState registry synchronized with server");
        });
        
        _bridge.RegisterHandler<ChunkPacket>(packet => 
        { 
            Console.WriteLine($"Received chunk packet from server at ({packet.Chunk.GetWorldPosition().X}, {packet.Chunk.GetWorldPosition().Z})");
            
            // Store chunk in local world
            var chunkPos = packet.Chunk.GetChunkPosition();
            _world.SetChunk((int)chunkPos.X, 0, (int)chunkPos.Y, packet.Chunk);
            Console.WriteLine("Stored chunk in local world");
            
            // Track loading progress
            _chunksLoaded++;
            if (_chunksRequested > 0 && _chunksLoaded >= _chunksRequested && !_initialLoadComplete)
            {
                _initialLoadComplete = true;
                Console.WriteLine($"Initial chunk loading complete! ({_chunksLoaded}/{_chunksRequested} chunks)");
            }
            else if (_chunksRequested > 0)
            {
                Console.WriteLine($"Loading progress: {_chunksLoaded}/{_chunksRequested} chunks");
            }
            
            // Notify handler if set
            _chunkUpdateHandler?.Invoke(packet.Chunk);
        });
        
        _bridge.RegisterHandler<ChunkRequestPacket>(_ => { Console.WriteLine("Received chunk request from server."); });
        
        _bridge.RegisterHandler<UpdateBlockPacket>(packet => 
        { 
            Console.WriteLine($"Received block update at ({packet.X}, {packet.Y}, {packet.Z})");
            
            // Update block in local world
            try
            {
                int chunkX = packet.X / 16;
                int chunkZ = packet.Z / 16;
                
                var chunk = _world.GetChunk(chunkX, 0, chunkZ);
                if (chunk != null)
                {
                    chunk.SetBlockStateId(packet.X % 16, packet.Y, packet.Z % 16, packet.BlockStateId);
                    Console.WriteLine("Updated block in local world");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating block: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// Requests a specific chunk from the server.
    /// </summary>
    /// <param name="chunkX">The X coordinate of the chunk to request.</param>
    /// <param name="chunkZ">The Z coordinate of the chunk to request.</param>
    public void RequestChunk(float chunkX, float chunkZ)
    {
        SendPacket(new ChunkRequestPacket(chunkX, chunkZ));
    }

    /// <summary>
    /// Sends a block update to the server.
    /// </summary>
    /// <param name="x">The world X coordinate of the block.</param>
    /// <param name="y">The world Y coordinate of the block.</param>
    /// <param name="z">The world Z coordinate of the block.</param>
    /// <param name="blockStateId">The new block state ID.</param>
    public void UpdateBlock(int x, int y, int z, ushort blockStateId)
    {
        SendPacket(new UpdateBlockPacket(x, y, z, blockStateId));
    }
}