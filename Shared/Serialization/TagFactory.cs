using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Serialization;

public static class TagFactory
{
    public static Tag Create(TagType type) => type switch
    {
        TagType.Byte => new TagByte(),
        TagType.Short => new TagShort(),
        TagType.Int => new TagInt(),
        TagType.Long => new TagLong(),
        TagType.Float => new TagFloat(),
        TagType.Double => new TagDouble(),
        TagType.String => new TagString(),
        TagType.List => new TagList(null, TagType.End),
        TagType.Compound => new TagCompound(),
        _ => throw new NotSupportedException($"Unknown Tag Type: {type}")
    };
}