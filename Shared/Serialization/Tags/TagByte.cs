using System.Text;

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
        writer.Write(Value);
    }

    public override void Read(BinaryReader reader)
    {
        int nameLen = reader.ReadInt32();
        if (nameLen > 0)
            Name = Encoding.UTF8.GetString(reader.ReadBytes(nameLen));
        else
            Name = null;
        Value = reader.ReadByte();
    }
    
    public override string ToString() => $"{{ {Name ?? "(unnamed)"} : {Value} }}";
}