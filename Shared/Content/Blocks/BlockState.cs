namespace VoxelForge.Shared.Content.Blocks;

/// <summary>
/// Represents a specific state of a block with optional properties.
/// For example, a door block might have "open" and "facing" properties.
/// BlockStates are immutable - use With() to create modified copies.
/// </summary>
public class BlockState
{
    /// <summary>
    /// Gets the block type this state represents.
    /// </summary>
    public Block Block { get; }
    
    private readonly Dictionary<string, object> _properties;

    /// <summary>
    /// Initializes a new instance of the BlockState class.
    /// </summary>
    /// <param name="block">The block type this state represents.</param>
    /// <param name="properties">Optional dictionary of property names to values.</param>
    public BlockState(Block block, Dictionary<string, object>? properties = null)
    {
        Block = block;
        _properties = properties ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// Checks if this block state has a property with the specified name.
    /// </summary>
    /// <param name="name">The name of the property to check.</param>
    /// <returns>True if the property exists, false otherwise.</returns>
    public bool HasProperty(string name) => _properties.ContainsKey(name);

    /// <summary>
    /// Gets the value of a property with the specified name and type.
    /// </summary>
    /// <typeparam name="T">The expected type of the property value.</typeparam>
    /// <param name="name">The name of the property to retrieve.</param>
    /// <returns>The property value.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the property does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the property value is not of type T.</exception>
    public T Get<T>(string name)
    {
        if (!_properties.TryGetValue(name, out var value))
            throw new KeyNotFoundException($"Property '{name}' not found.");
        if (value is T tValue)
            return tValue;
        throw new InvalidOperationException($"Property '{name}' is not of type {typeof(T).Name} (actual type: {value?.GetType().Name ?? "null"}).");
    }

    /// <summary>
    /// Creates a new BlockState with the specified property set to a new value.
    /// This method does not modify the current instance.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="name">The name of the property to set.</param>
    /// <param name="value">The new value for the property.</param>
    /// <returns>A new BlockState with the updated property.</returns>
    public BlockState With<T>(string name, T value)
    {
        var newProperties = new Dictionary<string, object>(_properties);
        newProperties[name] = value; // replaces or adds
        return new BlockState(Block, newProperties);
    }

    /// <summary>
    /// Returns a string representation of this block state.
    /// Format: "block_id" or "block_id[prop1=value1, prop2=value2]" if properties exist.
    /// </summary>
    /// <returns>A string representation of this block state.</returns>
    public override string ToString()
    {
        if (_properties.Count == 0) return Block.Id;
        var props = string.Join(", ", _properties.Select(kv => $"{kv.Key}={kv.Value}"));
        return $"{Block.Id}[{props}]";
    }
}
