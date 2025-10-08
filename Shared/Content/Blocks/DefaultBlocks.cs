namespace VoxelForge.Shared.Content.Blocks;

public static class DefaultBlocks
{
    public static readonly Block Air = new AirBlock();
    public static readonly Block Stone = new StoneBlock();
    public static readonly Block Grass = new GrassBlock();
    public static readonly Block Dirt = new DirtBlock();

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
