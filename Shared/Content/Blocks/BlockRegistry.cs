namespace VoxelForge.Shared.Content.Blocks;

using VoxelForge.Shared.Registry;

/// <summary>
/// Static registry for all block types in VoxelForge.
/// Provides centralized registration and lookup of block types by their string IDs.
/// </summary>
public static class BlockRegistry
{
    private static readonly Registry<Block> Registry = new();

    /// <summary>
    /// Registers a block type in the registry.
    /// The block is registered using its Id property.
    /// </summary>
    /// <param name="block">The block to register.</param>
    public static void Register(Block block) => Registry.Register(block.Id, block);

    /// <summary>
    /// Gets a block type by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the block to retrieve.</param>
    /// <returns>The block if found, otherwise null.</returns>
    public static Block? Get(string id) => Registry.Get(id);

    /// <summary>
    /// Gets all registered block types.
    /// </summary>
    /// <returns>A collection of all registered blocks.</returns>
    public static IEnumerable<Block> GetAll() => Registry.GetAll();
}