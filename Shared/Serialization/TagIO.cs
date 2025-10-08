using System.IO.Compression;

namespace VoxelForge.Shared.Serialization;

public static class TagIO
{
    public static void Write(Tag root, Stream stream, bool compress = true)
    {
        Stream target = compress ? new GZipStream(stream, CompressionLevel.SmallestSize) : stream;
        using var writer = new BinaryWriter(target);
        writer.Write((byte)root.Type);
        writer.Write(root.Name ?? "");
        root.Write(writer);
    }

    public static Tag Read(Stream stream, bool compressed = true)
    {
        Stream source = compressed ? new GZipStream(stream, CompressionMode.Decompress) : stream;
        using var reader = new BinaryReader(source);
        TagType type = (TagType)reader.ReadByte();
        string name = reader.ReadString();
        Tag tag = TagFactory.Create(type);
        tag.Name = name;
        tag.Read(reader);
        return tag;
    }
}