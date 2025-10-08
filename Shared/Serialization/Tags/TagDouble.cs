namespace VoxelForge.Shared.Serialization.Tags;

public class TagDouble : Tag
{
    public double Value { get; set; }
    public override TagType Type => TagType.Double;
    
    public TagDouble(double value = 0, string? name = null)
    {
        Value = value;
        Name = name;
    }
    
    public override void Write(BinaryWriter writer) => writer.Write(Value);
    public override void Read(BinaryReader reader) => Value = reader.ReadDouble();
    
    public override string ToString() => $"{{ {Name ?? "(unnamed)"} : {Value} }}";
}