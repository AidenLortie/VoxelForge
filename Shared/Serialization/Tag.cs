namespace VoxelForge.Shared.Serialization;

public abstract class Tag
{
    public string? Name { get; set; }
    public abstract TagType Type { get; }
    

    public abstract void Write(BinaryWriter writer);
    public abstract void Read(BinaryReader reader);
}