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
        // TagType.End is used here as a sentinel value to indicate that the list's element type is not yet set.
        // This matches the NBT specification, where an empty list has TagType.End as its type.
        TagType.List => new TagList(null, TagType.End),
        TagType.Compound => new TagCompound(),
        TagType.IntArray => new TagIntArray(),
        TagType.LongArray => new TagLongArray(),
        TagType.ByteArray => new TagByteArray(),
        _ => throw new NotSupportedException($"Unknown Tag Type: {type}")
    };
}