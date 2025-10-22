namespace VoxelForge.Shared.Content.Blocks;

using VoxelForge.Shared.Registry;



public static class BlockRegistry
{
    private static readonly Registry<Block> Registry = new();

    public static void Register(Block block) => Registry.Register(block.Id, block);

    public static Block? Get(string id) => Registry.Get(id);

    public static IEnumerable<Block> GetAll() => Registry.GetAll();
}