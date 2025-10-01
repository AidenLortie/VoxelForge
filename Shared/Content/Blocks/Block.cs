namespace VoxelForge.Shared.Content.Blocks;

public abstract class Block
{
    public string Id { get; }
    public virtual BlockState DefaultState() => new BlockState(this);
    

}
