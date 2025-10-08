using System.Text;

namespace VoxelForge.Shared.Serialization.Tags;

public class TagByteArray : Tag
{
    public byte[] Value { get; set; }
    public TagByteArray(byte[] value, string? name = null)
    {
        Value = value;
        Name = name;
    }

    public TagByteArray(string? name = null)
    {
        Value = new byte[1];
        Name = name;
    }
    public override TagType Type => TagType.ByteArray;

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
        writer.Write(Value.Length);
        writer.Write(Value);
    }

    public override void Read(BinaryReader reader)
    {
        int nameLen = reader.ReadInt32();
        if (nameLen > 0)
            Name = Encoding.UTF8.GetString(reader.ReadBytes(nameLen));
        else
            Name = null;
        int length = reader.ReadInt32();
        Value = reader.ReadBytes(length);
    }
    
    public override string ToString() => $"{{ {Name ?? "(unnamed)"} : {Value.Length} bytes }}";
}