using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Networking.Packets;

public class ChunkRequestPacket : Packet
{
    public override string Id => "ChunkRequest";
    
    public float ChunkX { get; set; }
    public float ChunkZ { get; set; }

    public ChunkRequestPacket()
    {
    }

    public ChunkRequestPacket(float chunkX, float chunkZ)
    {
        ChunkX = chunkX;
        ChunkZ = chunkZ;
    }

    public override TagCompound Write()
    {
        return new TagCompound(Id)
        {
            ["ChunkX"] = new TagFloat(ChunkX),
            ["ChunkZ"] = new TagFloat(ChunkZ)
        };
    }

    public override void Read(TagCompound tag)
    {
        ChunkX = (tag["ChunkX"] as TagFloat)?.Value ?? 0;
        ChunkZ = (tag["ChunkZ"] as TagFloat)?.Value ?? 0;
    }
}
