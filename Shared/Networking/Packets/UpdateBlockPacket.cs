using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Networking.Packets;

public class UpdateBlockPacket : Packet
{
    public override string Id => "UpdateBlock";
    
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public ushort BlockStateId { get; set; }

    public UpdateBlockPacket()
    {
    }

    public UpdateBlockPacket(int x, int y, int z, ushort blockStateId)
    {
        X = x;
        Y = y;
        Z = z;
        BlockStateId = blockStateId;
    }

    public override TagCompound Write()
    {
        return new TagCompound(Id)
        {
            ["X"] = new TagInt(X),
            ["Y"] = new TagInt(Y),
            ["Z"] = new TagInt(Z),
            ["BlockStateId"] = new TagShort((short)BlockStateId)
        };
    }

    public override void Read(TagCompound tag)
    {
        X = (tag["X"] as TagInt)?.Value ?? 0;
        Y = (tag["Y"] as TagInt)?.Value ?? 0;
        Z = (tag["Z"] as TagInt)?.Value ?? 0;
        BlockStateId = (ushort)((tag["BlockStateId"] as TagShort)?.Value ?? 0);
    }
}
