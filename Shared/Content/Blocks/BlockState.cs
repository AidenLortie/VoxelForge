namespace VoxelForge.Shared.Content.Blocks;

/// Represents a specific state of a block with optional properties.
/// For example, a door block might have "open" and "facing" properties.
/// BlockStates are immutable - use With() to create modified copies.
public class BlockState
{
    /// Gets the block type this state represents.
    public Block Block { get; }
    
    private readonly Dictionary<string, object> _properties;

    /// Initializes a new instance of the BlockState class.
    public BlockState(Block block, Dictionary<string, object>? properties = null)
    {
        Block = block;
        _properties = properties ?? new Dictionary<string, object>();
    }

    /// Checks if this block state has a property with the specified name.
    public bool HasProperty(string name) => _properties.ContainsKey(name);

    /// Gets the value of a property with the specified name and type.
    public T Get<T>(string name)
    {
        if (!_properties.TryGetValue(name, out var value))
            throw new KeyNotFoundException($"Property '{name}' not found.");
        if (value is T tValue)
            return tValue;
        throw new InvalidOperationException($"Property '{name}' is not of type {typeof(T).Name} (actual type: {value?.GetType().Name ?? "null"}).");
    }

    /// Creates a new BlockState with the specified property set to a new value.
    /// This method does not modify the current instance.
    public BlockState With<T>(string name, T value)
    {
        var newProperties = new Dictionary<string, object>(_properties);
        newProperties[name] = value; // replaces or adds
        return new BlockState(Block, newProperties);
    }

    /// Returns a string representation of this block state.
    /// Format: "block_id" or "block_id[prop1=value1, prop2=value2]" if properties exist.
    public override string ToString()
    {
        if (_properties.Count == 0) return Block.Id;
        var props = string.Join(", ", _properties.Select(kv => $"{kv.Key}={kv.Value}"));
        return $"{Block.Id}[{props}]";
    }
}
