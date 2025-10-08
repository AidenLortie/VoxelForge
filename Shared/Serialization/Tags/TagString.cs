using System.Text;

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
        if (Name != null)
        {
            var nameBytes = Encoding.UTF8.GetBytes(Name);
            writer.Write(nameBytes.Length);
            writer.Write(nameBytes);
        }
        else
        {
            writer.Write(0);
        }
        var bytes = Encoding.UTF8.GetBytes(Value);
        writer.Write(bytes.Length);
        writer.Write(bytes);
    }

    public override void Read(BinaryReader reader)
    {
        int nameLen = reader.ReadInt32();
        if (nameLen > 0)
            Name = Encoding.UTF8.GetString(reader.ReadBytes(nameLen));
        else
            Name = null;
        int length = reader.ReadInt32();
        var bytes = reader.ReadBytes(length);
        Value = Encoding.UTF8.GetString(bytes);
    }
}