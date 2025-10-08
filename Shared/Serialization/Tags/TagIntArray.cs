using System.Text;

namespace VoxelForge.Shared.Serialization.Tags;

public class TagIntArray : Tag
{
    public int[] Value { get; set; }
    public TagIntArray(int[] value, string? name = null)
    {
        Value = value;
        Name = name;
    }

    public TagIntArray(string? name = null)
    {
        Value = new int[1];
        Name = name;
    }
    public override TagType Type => TagType.IntArray;

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
        foreach (int i in Value)
        {
            writer.Write(i);
        }
    }

    public override void Read(BinaryReader reader)
    {
        int nameLen = reader.ReadInt32();
        if (nameLen > 0)
            Name = Encoding.UTF8.GetString(reader.ReadBytes(nameLen));
        else
            Name = null;
        int length = reader.ReadInt32();
        Value = new int[length];
        for (int i = 0; i < length; i++)
        {
            Value[i] = reader.ReadInt32();
        }
    }
    
    public override string ToString() => $"{{ {Name ?? "(unnamed)"} : {Value.Length * sizeof(int)} bytes }}";
}