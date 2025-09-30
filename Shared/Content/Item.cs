namespace VoxelForge.Shared.Content;

public class Item
{
    public string Id { get; }
    
    protected Item(string id) => Id = id;
}
