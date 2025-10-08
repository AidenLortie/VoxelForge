using System.Net;
using System.Net.Sockets;
using VoxelForge.Shared.Content.Blocks;
using VoxelForge.Shared.Networking;
using VoxelForge.Shared.Networking.NetworkBridge;
using VoxelForge.Shared.Networking.Packets;
using VoxelForge.Shared.Registry;
using VoxelForge.Shared.World;

namespace VoxelForge.Client;

public class Client
{
    private readonly INetworkBridge _bridge;
    private readonly World _world;

    public Client(INetworkBridge bridge)
    {
        _bridge = bridge;
        
        // Initialize default blocks
        DefaultBlocks.Initialize();
        
        // Create a local world
        _world = new World(1, 1, 1);
    }

    public static void Main()
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

    public void SendPacket(Packet packet)
    {
        _bridge.Send(packet);
    }
    
    public void Poll()
    {
        _bridge.Poll();
    }

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

    public void RequestChunk(float chunkX, float chunkZ)
    {
        SendPacket(new ChunkRequestPacket(chunkX, chunkZ));
    }

    public void UpdateBlock(int x, int y, int z, ushort blockStateId)
    {
        SendPacket(new UpdateBlockPacket(x, y, z, blockStateId));
    }
}