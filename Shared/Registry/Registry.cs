namespace VoxelForge.Shared.Registry;

/// <summary>
/// Generic implementation of IRegistry for managing a collection of entries by string identifiers.
/// </summary>
/// <typeparam name="T">The type of entries stored in this registry.</typeparam>
public class Registry<T> : IRegistry<T>
{
    private readonly Dictionary<string, T> _entries = new();
    
    /// <summary>
    /// Registers an entry with the specified identifier.
    /// If an entry with the same ID already exists, it will be replaced.
    /// </summary>
    /// <param name="id">The unique identifier for the entry.</param>
    /// <param name="entry">The entry to register.</param>
    public void Register(string id, T entry)
    {
        _entries[id] = entry;
    }

    /// <summary>
    /// Retrieves an entry by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entry to retrieve.</param>
    /// <returns>The entry if found, otherwise null.</returns>
    public T? Get(string id)
    {
        _entries.TryGetValue(id, out var entry);
        return entry;
    }

    /// <summary>
    /// Gets all registered entries.
    /// </summary>
    /// <returns>A collection of all registered entries.</returns>
    public IEnumerable<T> GetAll()
    {
        return _entries.Values;
    }
}