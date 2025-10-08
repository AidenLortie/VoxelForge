using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Networking;

public abstract class Packet
{
    public abstract string Id { get; }
    
    // Convert to and from TNB tags
    public abstract TagCompound Write();
    public abstract void Read(TagCompound compound);
}
