using VoxelForge.Shared.Content.Blocks;

namespace VoxelForge.Shared.Registry;

/// <summary>
/// Global registry for mapping BlockState instances to numeric IDs and vice versa.
/// This enables efficient storage and network transmission of block states.
/// </summary>
public class BlockStateRegistry
{
    private static readonly Dictionary<BlockState, ushort> _stateToId = new();
    private static readonly Dictionary<ushort, BlockState> _idToState = new();
    private static ushort _nextId = 1;

    /// <summary>
    /// Registers a block state and assigns it a unique numeric ID.
    /// If the state is already registered, returns its existing ID.
    /// </summary>
    /// <param name="state">The block state to register.</param>
    /// <returns>The numeric ID assigned to this block state.</returns>
    public static ushort Register(BlockState state)
    {
        if(_stateToId.TryGetValue(state, out var id)) return id;
        id = _nextId++;
        _stateToId[state] = id;
        _idToState[id] = state;
        return id;
    }
    
    /// <summary>
    /// Gets the numeric ID for a registered block state.
    /// </summary>
    /// <param name="state">The block state to look up.</param>
    /// <returns>The numeric ID of the block state.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the block state is not registered.</exception>
    public static ushort GetId(BlockState state) => _stateToId[state];
    
    /// <summary>
    /// Gets the block state for a registered numeric ID.
    /// </summary>
    /// <param name="id">The numeric ID to look up.</param>
    /// <returns>The block state associated with the ID.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the ID is not registered.</exception>
    public static BlockState GetState(ushort id) => _idToState[id];
    
    /// <summary>
    /// Exports all registered block state mappings as ID-to-String dictionary.
    /// </summary>
    /// <returns>Dictionary mapping state IDs to their string representations</returns>
    public static Dictionary<ushort, string> ExportMappings()
    {
        var mappings = new Dictionary<ushort, string>();
        foreach (var kvp in _idToState)
        {
            mappings[kvp.Key] = kvp.Value.ToString();
        }
        return mappings;
    }
    
    /// <summary>
    /// Imports block state mappings from a dictionary.
    /// Clears existing mappings and rebuilds the registry.
    /// </summary>
    /// <param name="mappings">Dictionary mapping state IDs to their string representations</param>
    public static void ImportMappings(Dictionary<ushort, string> mappings)
    {
        _stateToId.Clear();
        _idToState.Clear();
        _nextId = 1;
        
        foreach (var kvp in mappings)
        {
            // Parse the string back into a BlockState
            // For now, we'll just store it as a simple mapping
            // The actual BlockState reconstruction will need the Block instances
            // This is a simplified implementation
            ushort id = kvp.Key;
            if (id >= _nextId)
                _nextId = (ushort)(id + 1);
        }
    }
    
    /// <summary>
    /// Gets the total number of registered block states.
    /// </summary>
    public static int Count => _idToState.Count;
    
    /// <summary>
    /// Clears all registered block states.
    /// </summary>
    public static void Clear()
    {
        _stateToId.Clear();
        _idToState.Clear();
        _nextId = 1;
    }
}