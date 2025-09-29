namespace VoxelForge.Shared.Content;

public abstract class Block
{
    public string Id { get; }

    protected Block(string id) => Id = id;
}
