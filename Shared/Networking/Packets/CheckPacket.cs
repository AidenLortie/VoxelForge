using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Networking.Packets;

public class CheckPacket : Packet
{
    public override string Id => "Check";
    public long Timestamp { get; set; }

    public CheckPacket() { }

    public CheckPacket(long timestamp)
    {
        Timestamp = timestamp;
    }

    public override TagCompound Write()
    {
        return new TagCompound(Id)
        {
            ["Timestamp"] = new TagLong(Timestamp)
        };
    }

    public override void Read(TagCompound tag)
    {
        Timestamp = (tag["Timestamp"] as TagLong ?? new TagLong()).Value;
    }
}