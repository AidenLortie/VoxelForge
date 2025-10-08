namespace VoxelForge.Shared.Content.Blocks;

/// <summary>
/// Provides default block types for VoxelForge.
/// Call Initialize() on startup to register all default blocks and their states.
/// </summary>
public static class DefaultBlocks
{
    /// <summary>Gets the Air block (empty space).</summary>
    public static readonly Block Air = new AirBlock();
    
    /// <summary>Gets the Stone block.</summary>
    public static readonly Block Stone = new StoneBlock();
    
    /// <summary>Gets the Grass block.</summary>
    public static readonly Block Grass = new GrassBlock();
    
    /// <summary>Gets the Dirt block.</summary>
    public static readonly Block Dirt = new DirtBlock();

    /// <summary>
    /// Initializes and registers all default blocks and their default states.
    /// This must be called before using any blocks in the game.
    /// </summary>
    public static void Initialize()
    {
        BlockRegistry.Register(Air);
        BlockRegistry.Register(Stone);
        BlockRegistry.Register(Grass);
        BlockRegistry.Register(Dirt);
        
        // Register default block states
        VoxelForge.Shared.Registry.BlockStateRegistry.Register(Air.DefaultState());
        VoxelForge.Shared.Registry.BlockStateRegistry.Register(Stone.DefaultState());
        VoxelForge.Shared.Registry.BlockStateRegistry.Register(Grass.DefaultState());
        VoxelForge.Shared.Registry.BlockStateRegistry.Register(Dirt.DefaultState());
    }
}
