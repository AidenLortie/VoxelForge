namespace VoxelForge.Shared.Networking;

public interface INetworkBridge
{
    void Send(Packet packet);
    void RegisterHandler<T>(Action<T> handler) where T : Packet;
    void Poll(); // Process queued packets
}