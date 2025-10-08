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
}