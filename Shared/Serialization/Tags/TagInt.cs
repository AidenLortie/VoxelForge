namespace VoxelForge.Shared.Serialization.Tags;

public class TagInt : Tag
{
    public int Value { get; set; }
    public override TagType Type => TagType.Int;
    
    public TagInt(int value = 0, string? name = null)
    {
        Value = value;
        Name = name;
    }

    public override void Write(BinaryWriter writer) => writer.Write(Value);

    public override void Read(BinaryReader reader) => Value = reader.ReadInt32();
    
    public override string ToString() => $"{{ {Name ?? "(unnamed)"} : {Value} }}";
}