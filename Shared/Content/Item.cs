namespace VoxelForge.Shared.Content;

public abstract class Item
{
    public string Id { get; }
    
    protected Item(string id) => Id = id;
}
