namespace VoxelForge.Shared.Content;

public abstract class Entity
{
    public string Id { get; }
    
    protected Entity(string id) => Id = id;
}