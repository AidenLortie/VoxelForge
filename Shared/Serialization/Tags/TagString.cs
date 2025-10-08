namespace VoxelForge.Shared.Serialization.Tags;

public class TagString : Tag
{
    public string Value { get; set; }
    public override TagType Type => TagType.String;
    public TagString(string value = "", string? name = null)
    {
        Value = value;
        Name = name;
    }
    public override void Write(BinaryWriter writer)
    {
        // write the length of the string as an int (4 bytes)
        writer.Write(Value.Length);
        // write the string
        writer.Write(Value);
    }

    public override void Read(BinaryReader reader)
    {
        // read the length of the string
        int length = reader.ReadInt32();
        // read the string
        Value = new string(reader.ReadChars(length));
    }
}