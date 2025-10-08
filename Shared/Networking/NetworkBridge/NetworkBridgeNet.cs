using System.Net.Sockets;
using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Networking.NetworkBridge;

public class NetworkBridgeNet : INetworkBridge
{
    private readonly NetworkStream _stream;
    private readonly BinaryWriter _writer;
    private readonly BinaryReader _reader;
    private readonly Dictionary<string, Func<Packet>> _packetRegistry;
    private readonly Dictionary<Type, Delegate> _handlers = new();

    public NetworkBridgeNet(NetworkStream stream, Dictionary<string, Func<Packet>> packetRegistry)
    {
        _stream = stream;
        _writer = new BinaryWriter(_stream);
        _reader = new BinaryReader(_stream);
        _packetRegistry = packetRegistry;
    }

    public void Send(Packet packet)
    {
        TagCompound tag = packet.Write();
        tag.Write(_writer);
        _writer.Flush();
    }

    public void RegisterHandler<T>(Action<T> handler) where T : Packet
    {
        _handlers[typeof(T)] = handler;
    }

    public void Poll()
    {
        if (_stream.DataAvailable)
        {
            TagCompound tag = new TagCompound();
            tag.Read(_reader);
            string packetId = tag.Name;
            if (_packetRegistry.TryGetValue(packetId, out var packetFactory))
            {
                Packet packet = packetFactory();
                packet.Read(tag);
                if (_handlers.TryGetValue(packet.GetType(), out var handler))
                {
                    ((Action<Packet>)handler).Invoke(packet);
                }
            }
        }
    }
}