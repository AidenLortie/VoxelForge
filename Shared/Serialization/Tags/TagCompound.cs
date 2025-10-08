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
        foreach (var tag in _tags.Values)
        {
            writer.Write((byte)tag.Type);
            writer.Write(tag.Name ?? "");
            tag.Write(writer);
        }

        writer.Write((byte)TagType.End);
    }

    public override void Read(BinaryReader reader)
    {
        _tags.Clear();
        while (true)
        {
            TagType type = (TagType)reader.ReadByte();
            if (type == TagType.End) break;
                
            string name = reader.ReadString();
            Tag tag = TagFactory.Create(type);
            tag.Name = name;
            tag.Read(reader);
            _tags[name] = tag;
        }
    }

    public override string ToString() => $"{Name ?? "(root)"} = {{ {_tags.Count} tags }}";
}