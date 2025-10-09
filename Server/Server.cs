using System.Net;
using System.Net.Sockets;
using System.Numerics;
using VoxelForge.Shared.Content.Blocks;
using VoxelForge.Shared.Networking;
using VoxelForge.Shared.Networking.NetworkBridge;
using VoxelForge.Shared.Networking.Packets;
using VoxelForge.Shared.Registry;
using VoxelForge.Shared.World;

namespace VoxelForge.Server;

/// <summary>
/// Represents the VoxelForge game server.
/// Manages the authoritative world state and handles client connections and requests.
/// </summary>
public class Server
{
    private readonly INetworkBridge _bridge;
    private readonly World _world;

    /// <summary>
    /// Initializes a new Server instance with the specified network bridge.
    /// Creates a 16x16 chunk world with generated terrain, or loads from disk if available.
    /// </summary>
    /// <param name="bridge">The network bridge to use for communication with clients.</param>
    /// <param name="seed">The seed to use for world generation. Defaults to 12345.</param>
    public Server(INetworkBridge bridge, int seed = 12345)
    {
        _bridge = bridge;
        
        // Initialize default blocks
        DefaultBlocks.Initialize();
        
        // Try to load world from disk first
        var serializer = new WorldSerializer();
        _world = serializer.Load(seed);
        
        if (_world != null)
        {
            Console.WriteLine($"Loaded existing world for seed {seed}");
        }
        else
        {
            Console.WriteLine($"Generating new 16x16 chunk world with seed {seed}...");
            
            // Create a 16x16 chunk world
            _world = new World(16, 1, 16);
            var generator = new WorldGenerator(seed);
            
            // Generate all chunks
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    var chunk = generator.GenerateChunk(x, z);
                    _world.SetChunk(x, 0, z, chunk);
                }
            }
            
            Console.WriteLine("World generation complete!");
            
            // Save the generated world
            serializer.Save(_world, seed);
        }
        
        // Register packet handlers
        bridge.RegisterHandler<CheckPacket>(packet =>
        {
            Console.WriteLine("Received check packet from client. with timestamp: " + packet.Timestamp);
        });
        
        bridge.RegisterHandler<ChunkRequestPacket>(packet =>
        {
            Console.WriteLine($"Received chunk request for ({packet.ChunkX}, {packet.ChunkZ})");
            HandleChunkRequest(packet);
        });
        
        bridge.RegisterHandler<UpdateBlockPacket>(packet =>
        {
            Console.WriteLine($"Received block update at ({packet.X}, {packet.Y}, {packet.Z}) with state {packet.BlockStateId}");
            HandleBlockUpdate(packet);
        });
    }

    /// <summary>
    /// Handles a chunk request from a client by sending the requested chunk if it exists.
    /// </summary>
    /// <param name="packet">The chunk request packet from the client.</param>
    private void HandleChunkRequest(ChunkRequestPacket packet)
    {
        int chunkX = (int)packet.ChunkX;
        int chunkZ = (int)packet.ChunkZ;
        
        // Check if chunk is within world bounds
        if (chunkX >= 0 && chunkX < 16 && chunkZ >= 0 && chunkZ < 16)
        {
            var chunk = _world.GetChunk(chunkX, 0, chunkZ);
            if (chunk != null)
            {
                _bridge.Send(new ChunkPacket(chunk));
                Console.WriteLine($"Sent chunk ({chunkX}, {chunkZ}) to client");
            }
        }
        else
        {
            Console.WriteLine($"Chunk ({chunkX}, {chunkZ}) is out of bounds");
        }
    }

    /// <summary>
    /// Handles a block update from a client by updating the block in the server's world.
    /// </summary>
    /// <param name="packet">The block update packet from the client.</param>
    private void HandleBlockUpdate(UpdateBlockPacket packet)
    {
        try
        {
            // Determine which chunk this block belongs to
            int chunkX = packet.X / 16;
            int chunkZ = packet.Z / 16;
            
            // For simplicity, we only have one chunk at 0,0
            if (chunkX == 0 && chunkZ == 0)
            {
                var chunk = _world.GetChunk(0, 0, 0);
                if (chunk != null)
                {
                    chunk.SetBlockStateId(packet.X % 16, packet.Y, packet.Z % 16, packet.BlockStateId);
                    Console.WriteLine($"Updated block at ({packet.X}, {packet.Y}, {packet.Z})");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating block: {ex.Message}");
        }
    }

    /// <summary>
    /// Runs the server main loop asynchronously.
    /// Sends initial world state to clients and processes packets continuously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RunAsync()
    {
        Console.WriteLine("Server started. Waiting for packets...");
        
        // Send BlockState registry first
        var registryMappings = BlockStateRegistry.ExportMappings();
        _bridge.Send(new BlockStateRegistryPacket(registryMappings));
        Console.WriteLine($"Sent BlockState registry ({registryMappings.Count} states)");
        
        // Send initial world sync - send all chunks to client
        foreach (var chunk in _world.GetAllChunks())
        {
            _bridge.Send(new ChunkPacket(chunk));
            Console.WriteLine("Sent initial chunk to client");
        }
        
        while (true)
        {
            _bridge.Poll(); // Let bridge process queued data
            
            _bridge.Send(new CheckPacket());

            // Handle server logic here (sending ticks, world updates, etc.)
            await Task.Delay(100);
        }
    }

    /// <summary>
    /// Main entry point for the server application.
    /// Listens on port 25565 and accepts multiple client connections.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Main()
    {
        Console.WriteLine("VoxelForge multi-client server starting...");
        
        var multiServer = new MultiClientServer(12345); // Fixed seed
        await multiServer.StartAsync(25565);
    }
}