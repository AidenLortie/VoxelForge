using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Networking;

/// <summary>
/// Base class for all network packets in VoxelForge.
/// Packets are serialized to Tag format for network transmission.
/// </summary>
public abstract class Packet
{
    /// <summary>
    /// Gets the unique identifier for this packet type (e.g., "check", "chunk").
    /// Used for packet routing and deserialization.
    /// </summary>
    public abstract string Id { get; }
    
    /// <summary>
    /// Serializes this packet to a TagCompound for network transmission.
    /// </summary>
    /// <returns>A TagCompound containing the packet data.</returns>
    public abstract TagCompound Write();
    
    /// <summary>
    /// Deserializes packet data from a TagCompound received over the network.
    /// </summary>
    /// <param name="compound">The TagCompound containing the packet data.</param>
    public abstract void Read(TagCompound compound);
}
