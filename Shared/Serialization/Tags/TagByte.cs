namespace VoxelForge.Shared.Serialization.Tags;

public class TagByte : Tag
{
    public byte Value { get; set; }
    public override TagType Type => TagType.Byte;
    
    public TagByte(byte value = 0, string? name = null)
    {
        Value = value;
        Name = name;
    }
    
    public override void Write(BinaryWriter writer) => writer.Write(Value);

    public override void Read(BinaryReader reader) => Value = reader.ReadByte();
    
    public override string ToString() => $"{{ {Name ?? "(unnamed)"} : {Value} }}";
}