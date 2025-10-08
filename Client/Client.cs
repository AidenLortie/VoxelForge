using System.Net;
using System.Net.Sockets;
using VoxelForge.Shared.Content.Blocks;
using VoxelForge.Shared.Networking;
using VoxelForge.Shared.Networking.NetworkBridge;
using VoxelForge.Shared.Networking.Packets;
using VoxelForge.Shared.Registry;
using VoxelForge.Shared.World;

namespace VoxelForge.Client;

/// <summary>
/// Represents the VoxelForge game client.
/// Handles connection to the server, receiving world data, and sending player actions.
/// </summary>
public class Client
{
    private readonly INetworkBridge _bridge;
    private readonly World _world;

    /// <summary>
    /// Initializes a new Client instance with the specified network bridge.
    /// </summary>
    /// <param name="bridge">The network bridge to use for communication with the server.</param>
    public Client(INetworkBridge bridge)
    {
        _bridge = bridge;
        
        // Initialize default blocks
        DefaultBlocks.Initialize();
        
        // Create a local world
        _world = new World(1, 1, 1);
    }

    /// <summary>
    /// Main entry point for the client application.
    /// Connects to the server on localhost:25565 and enters the main game loop.
    /// </summary>
    /// <param name="args">Command line arguments. Use --rendering to enable OpenTK window.</param>
    public static void Main(string[] args)
    {
        // Check if rendering mode is enabled
        bool useRendering = args.Contains("--rendering");
        
        if (useRendering)
        {
            Console.WriteLine("Starting VoxelForge client with rendering enabled...");
            RunWithRendering();
        }
        else
        {
            Console.WriteLine("Starting VoxelForge client in console mode...");
            Console.WriteLine("Use --rendering flag to enable OpenTK window");
            RunConsoleMode();
        }
    }
    
    /// <summary>
    /// Runs the client with OpenTK rendering window.
    /// </summary>
    private static void RunWithRendering()
    {
        // Connect to server
        var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        try
        {
            clientSocket.Connect(IPAddress.Loopback, 25565);
            Console.WriteLine("Connected to server at localhost:25565");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to server: {ex.Message}");
            Console.WriteLine("Make sure the server is running on localhost:25565");
            return;
        }

        var stream = new NetworkStream(clientSocket);
        var bridgeNet = new NetworkBridgeNet(stream, PacketRegistry.Factories);

        var client = new Client(bridgeNet);

        // Start listening for incoming packets
        client.StartListening();

        // Request the initial chunk
        client.SendPacket(new ChunkRequestPacket(0, 0));
        
        // Create and run the rendering window
        using var window = new Rendering.GameWindow(client);
        window.Run();
        
        Console.WriteLine("Window closed");
    }
    
    /// <summary>
    /// Runs the client in console-only mode (original behavior).
    /// </summary>
    private static void RunConsoleMode()
    {
        // Connect to server
        var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(IPAddress.Loopback, 25565);

        var stream = new NetworkStream(clientSocket);
        var bridgeNet = new NetworkBridgeNet(stream, PacketRegistry.Factories);

        var client = new Client(bridgeNet);

        // Start listening for incoming packets
        client.StartListening();

        // Request the initial chunk
        client.SendPacket(new ChunkRequestPacket(0, 0));
        
        // Main loop
        while (true)
        {
            client.Poll();
            client.SendPacket(new CheckPacket());
            Task.Delay(100).Wait();
        }
    }

    /// <summary>
    /// Sends a packet to the server.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    public void SendPacket(Packet packet)
    {
        _bridge.Send(packet);
    }
    
    /// <summary>
    /// Polls the network bridge to process incoming packets.
    /// Should be called regularly in the game loop.
    /// </summary>
    public void Poll()
    {
        _bridge.Poll();
    }

    /// <summary>
    /// Registers packet handlers for all incoming packet types.
    /// Call this once after creating the client to begin processing server messages.
    /// </summary>
    public void StartListening()
    {
        _bridge.RegisterHandler<CheckPacket>(_ => { Console.WriteLine("Received check packet from server."); });
        
        _bridge.RegisterHandler<ChunkPacket>(packet => 
        { 
            Console.WriteLine($"Received chunk packet from server at ({packet.Chunk.GetWorldPosition().X}, {packet.Chunk.GetWorldPosition().Z})");
            
            // Store chunk in local world
            var chunkPos = packet.Chunk.GetChunkPosition();
            _world.SetChunk((int)chunkPos.X, 0, (int)chunkPos.Y, packet.Chunk);
            Console.WriteLine("Stored chunk in local world");
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