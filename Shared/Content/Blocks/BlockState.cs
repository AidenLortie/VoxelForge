namespace VoxelForge.Shared.Content.Blocks;

public class BlockState
{
    public Block Block { get; }
    private readonly Dictionary<string, object> _properties;

    public BlockState(Block block, Dictionary<string, object>? properties = null)
    {
        Block = block;
        _properties = properties ?? new Dictionary<string, object>();
    }

    public bool HasProperty(string name) => _properties.ContainsKey(name);

    public T Get<T>(string name)
    {
        if (!_properties.TryGetValue(name, out var value))
            throw new KeyNotFoundException($"Property '{name}' not found.");
        if (value is T tValue)
            return tValue;
        throw new InvalidOperationException($"Property '{name}' is not of type {typeof(T).Name} (actual type: {value?.GetType().Name ?? "null"}).");
    }

    public BlockState With<T>(string name, T value)
    {
        var clone = new BlockState(Block, new Dictionary<string, object>(_properties))
        {
            _properties = { [name] = value } // replaces or adds
        };
        return clone;
    }

    public override string ToString()
    {
        if (_properties.Count == 0) return Block.Id;
        var props = string.Join(", ", _properties.Select(kv => $"{kv.Key}={kv.Value}"));
        return $"{Block.Id}[{props}]";
    }
}
