namespace VoxelForge.Shared.Content.Blocks;

public abstract class Block
{
    public string Id { get; }
    public abstract BlockState DefaultState { get; }
    

}
