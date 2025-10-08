namespace VoxelForge.Shared.Serialization.Tags;

public class TagLong : Tag
{
    public long Value { get; set; }
    public override TagType Type => TagType.Long;
    public TagLong(long value = 0, string? name = null)
    {
        Value = value;
        Name = name;
    }
    
    public override void Write(BinaryWriter writer) => writer.Write(Value);

    public override void Read(BinaryReader reader) => Value = reader.ReadInt64();
}