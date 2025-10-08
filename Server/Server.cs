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

public class Server
{
    private readonly INetworkBridge _bridge;
    private readonly World _world;

    public Server(INetworkBridge bridge)
    {
        _bridge = bridge;
        
        // Initialize default blocks
        DefaultBlocks.Initialize();
        
        // Create a simple world with one chunk
        _world = new World(1, 1, 1);
        var chunk = new Chunk(Vector2.Zero);
        
        // Fill chunk with some test data
        for(int x = 0; x < 16; x++)
        {
            for(int z = 0; z < 16; z++)
            {
                for (int y = 0; y < 256; y++)
                {
                    if (y % 16 == 0)
                    {
                        chunk.SetBlockStateId(x, y, z, 1); // Stone
                    }
                    else
                    {
                        chunk.SetBlockStateId(x, y, z, 0); // Air
                    }
                }
            }
        }
        
        _world.SetChunk(0, 0, 0, chunk);
        
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

    private void HandleChunkRequest(ChunkRequestPacket packet)
    {
        // For now, just send the single chunk we have if it matches
        if (packet.ChunkX == 0 && packet.ChunkZ == 0)
        {
            var chunk = _world.GetChunk(0, 0, 0);
            if (chunk != null)
            {
                _bridge.Send(new ChunkPacket(chunk));
                Console.WriteLine("Sent chunk to client");
            }
        }
    }

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

    public async Task RunAsync()
    {
        Console.WriteLine("Server started. Waiting for packets...");
        
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

    public static async Task Main()
    {
        // Setup TCP listener
        var listener = new TcpListener(IPAddress.Loopback, 25565);
        listener.Start();
        Console.WriteLine("VoxelForge server listening on 25565...");

        var client = await listener.AcceptTcpClientAsync();
        Console.WriteLine("Client connected.");

        // Create bridge over network stream
        NetworkStream stream = client.GetStream();

        INetworkBridge bridgeNet = new NetworkBridgeNet(stream, PacketRegistry.Factories);
        
        // Create and run server
        var server = new Server(bridgeNet);
        await server.RunAsync();
    }
}