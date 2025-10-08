namespace VoxelForge.Shared.Networking;

/// <summary>
/// Interface for network communication between client and server.
/// Provides abstraction for sending packets, registering handlers, and processing incoming data.
/// </summary>
public interface INetworkBridge
{
    /// <summary>
    /// Sends a packet over the network.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    void Send(Packet packet);
    
    /// <summary>
    /// Registers a handler for a specific packet type.
    /// When a packet of type T is received, the handler will be invoked.
    /// </summary>
    /// <typeparam name="T">The type of packet to handle.</typeparam>
    /// <param name="handler">The handler function to invoke when a packet is received.</param>
    void RegisterHandler<T>(Action<T> handler) where T : Packet;
    
    /// <summary>
    /// Processes queued incoming packets.
    /// Should be called regularly in the game loop to handle received packets.
    /// </summary>
    void Poll();
}