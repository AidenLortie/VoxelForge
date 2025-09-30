namespace VoxelForge.Shared.Registry;

public class Registry<T> : IRegistry<T>
{
    private readonly Dictionary<string, T> _entries = new();
    
    public void Register(string id, T entry)
    {
        _entries[id] = entry;
    }

    public T? Get(string id)
    {
        _entries.TryGetValue(id, out var entry);
        return entry;
    }

    public IEnumerable<T> GetAll()
    {
        return _entries.Values;
    }
}