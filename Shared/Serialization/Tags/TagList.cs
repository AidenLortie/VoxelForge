using System.Collections;

namespace VoxelForge.Shared.Serialization.Tags;

public class TagList : Tag, IEnumerable<Tag>
{
    private readonly List<Tag> _tags = new();

    public override TagType Type => TagType.List;
    public TagType ElementType { get; private set; } = TagType.End;

    public TagList(string? name, TagType elementType, params Tag[] tags)
    {
        Name = name;
        ElementType = elementType;
        _tags.AddRange(tags);
    }

    public void Add(Tag tag)
    {
        if (tag.Type != ElementType)
        {
            throw new InvalidOperationException($"Expected {ElementType}, got {tag.Type}");
        }
        _tags.Add(tag);
    }
    
    public IEnumerator<Tag> GetEnumerator() => _tags.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)ElementType);
        writer.Write(_tags.Count);
        foreach (Tag tag in _tags)
        {
            tag.Write(writer);
        }
    }

    public override void Read(BinaryReader reader)
    {
        _tags.Clear();
        TagType type = (TagType)reader.ReadByte();
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            Tag tag = TagFactory.Create(type);
            tag.Read(reader);
            _tags.Add(tag);
        }
    }
    
    public override string ToString() => $"{Name ?? "(list)"}[{_tags.Count}]";

}