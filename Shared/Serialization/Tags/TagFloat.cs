namespace VoxelForge.Shared.Serialization.Tags;

public class TagFloat : Tag
{
    public float Value { get; set; }
    public override TagType Type => TagType.Float;
    
    public TagFloat(float value = 0f, string? name = null)
    {
        Value = value;
        Name = name;
    }
    public override void Write(BinaryWriter writer) => writer.Write(Value);
    public override void Read(BinaryReader reader) => Value = reader.ReadSingle();
    
    public override string ToString() => $"{{ {Name ?? "(unnamed)"} : {Value} }}";
}