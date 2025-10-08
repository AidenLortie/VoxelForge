using System.Collections.Concurrent;

namespace VoxelForge.Shared.Networking;

public class NetworkBridgeLocal : INetworkBridge
{
    private readonly ConcurrentQueue<Packet> _incoming = new();
    private readonly Dictionary<Type, Delegate> _handlers = new();
    private NetworkBridgeLocal? _remote;

    public void ConnectTo(NetworkBridgeLocal other)
    {
        _remote = other;
        other._remote = this;
    }

    public void Send(Packet packet)
    {
        _remote?._incoming.Enqueue(packet);
    }

    public void RegisterHandler<T>(Action<T> handler) where T : Packet
    {
        _handlers[typeof(T)] = handler;
    }

    public void Poll()
    {
        while (_incoming.TryDequeue(out var packet))
        {
            if (_handlers.TryGetValue(packet.GetType(), out var handler))
                handler.DynamicInvoke(packet);
        }
    }
}