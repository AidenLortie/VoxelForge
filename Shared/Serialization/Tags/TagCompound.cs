using System.Collections;

namespace VoxelForge.Shared.Serialization.Tags;

public class TagCompound : Tag, IEnumerable<KeyValuePair<string, Tag>>
{
    private readonly Dictionary<string, Tag> _tags = new();

    public override TagType Type => TagType.Compound;

    public TagCompound(string? name = null)
    {
        Name = name;
    }

    public Tag this[string key]
    {
        get => _tags[key];
        set
        {
            value.Name = key;
            _tags[key] = value;
        }
    }

    public void Add(string key, Tag value)
    {
        value.Name = key;
        _tags[key] = value;
    }

    public IEnumerator<KeyValuePair<string, Tag>> GetEnumerator() => _tags.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override void Write(BinaryWriter writer)
    {
        if (Name != null)
        {
            var nameBytes = System.Text.Encoding.UTF8.GetBytes(Name);
            writer.Write(nameBytes.Length);
            writer.Write(nameBytes);
        }
        else
        {
            writer.Write(0);
        }
        foreach (var tag in _tags.Values)
        {
            writer.Write((byte)tag.Type);
            if (tag.Name != null)
            {
                var childNameBytes = System.Text.Encoding.UTF8.GetBytes(tag.Name);
                writer.Write(childNameBytes.Length);
                writer.Write(childNameBytes);
            }
            else
            {
                writer.Write(0);
            }
            tag.Write(writer);
        }
        writer.Write((byte)TagType.End);
    }

    public override void Read(BinaryReader reader)
    {
        int nameLen = reader.ReadInt32();
        if (nameLen > 0)
            Name = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(nameLen));
        else
            Name = null;
        _tags.Clear();
        while (true)
        {
            TagType type = (TagType)reader.ReadByte();
            if (type == TagType.End)
                break;
            int childNameLen = reader.ReadInt32();
            string? childName = null;
            if (childNameLen > 0)
                childName = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(childNameLen));
            Tag tag = TagFactory.Create(type);
            tag.Name = childName;
            tag.Read(reader);
            _tags[childName ?? ""] = tag;
        }
    }

    public override string ToString() => $"{Name ?? "(root)"} = {{ {_tags.Count} tags }}";
}