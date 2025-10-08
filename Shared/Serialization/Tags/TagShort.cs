namespace VoxelForge.Shared.Serialization.Tags;

public class TagShort : Tag
{
    public short Value { get; set; }
    public override TagType Type => TagType.Short;
    public TagShort(short value = 0, string? name = null)
    {
        Value = value;
        Name = name;
    }
    public override void Write(BinaryWriter writer) => writer.Write(Value);
    public override void Read(BinaryReader reader) => Value = reader.ReadInt16();
}