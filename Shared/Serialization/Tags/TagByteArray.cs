namespace VoxelForge.Shared.Serialization.Tags;

public class TagByteArray : Tag
{
    public byte[] Value { get; set; }
    public TagByteArray(byte[] value, string? name = null)
    {
        Value = value;
        Name = name;
    }
    public override TagType Type => TagType.ByteArray;

    public override void Write(BinaryWriter writer)
    {
        writer.Write(Value.Length);
        writer.Write(Value);
    }

    public override void Read(BinaryReader reader)
    {
        int length = reader.ReadInt32();
        Value = reader.ReadBytes(length);
    }
    
    public override string ToString() => $"{{ {Name ?? "(unnamed)"} : {Value.Length} bytes }}";
}