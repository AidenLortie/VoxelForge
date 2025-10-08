using System.Net;
using System.Net.Sockets;
using VoxelForge.Shared.Networking;
using VoxelForge.Shared.Networking.NetworkBridge;
using VoxelForge.Shared.Networking.Packets;
using VoxelForge.Shared.Registry;

namespace VoxelForge.Client;

public class Client
{
    private readonly INetworkBridge _bridge;

    public Client(INetworkBridge bridge)
    {
        _bridge = bridge;
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

        // Example: send a check packet
        client.SendPacket(new CheckPacket());
        
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
        _bridge.RegisterHandler<ChunkPacket>(_ => { Console.WriteLine("Received chunk packet from server."); });
    }


}