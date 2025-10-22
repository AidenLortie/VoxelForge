namespace VoxelForge.Shared.Content.Blocks;

/// Base class for all block types in VoxelForge.
/// Blocks represent the basic building units of the voxel world (e.g., stone, dirt, grass).
public abstract class Block
{
    /// Gets the unique identifier for this block type (e.g., "stone", "grass").
    public string Id { get; }
    
    /// Gets the default state for this block. Override to provide custom default properties.
    public virtual BlockState DefaultState() => new BlockState(this);
    
    /// Initializes a new instance of the Block class with the specified identifier.
    protected Block(string id)
    {
        Id = id;
    }

}
