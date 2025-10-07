using VoxelForge.Shared.Content.Blocks;

namespace VoxelForge.Shared.Registry;

public class BlockStateRegistry
{
    private static readonly Dictionary<BlockState, ushort> _stateToId = new();
    private static readonly Dictionary<ushort, BlockState> _idToState = new();
    private static ushort _nextId = 1;

    public static ushort Register(BlockState state)
    {
        if(_stateToId.TryGetValue(state, out var id)) return id;
        id = _nextId++;
        _stateToId[state] = id;
        _idToState[id] = state;
        return id;
    }
    
    public static ushort GetId(BlockState state) => _stateToId[state];
    public static BlockState GetState(ushort id) => _idToState[id];
}