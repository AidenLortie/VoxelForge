namespace VoxelForge.Shared.Content.Blocks;

/// <summary>
/// Base class for all block types in VoxelForge.
/// Blocks represent the basic building units of the voxel world (e.g., stone, dirt, grass).
/// </summary>
public abstract class Block
{
    /// <summary>
    /// Gets the unique identifier for this block type (e.g., "stone", "grass").
    /// </summary>
    public string Id { get; }
    
    /// <summary>
    /// Gets the default state for this block. Override to provide custom default properties.
    /// </summary>
    /// <returns>A BlockState representing the default configuration of this block.</returns>
    public virtual BlockState DefaultState() => new BlockState(this);
    
    /// <summary>
    /// Initializes a new instance of the Block class with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier for this block type.</param>
    protected Block(string id)
    {
        Id = id;
    }

}
