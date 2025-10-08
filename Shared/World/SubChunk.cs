using System.Numerics;
using VoxelForge.Shared.Content.Blocks;
using VoxelForge.Shared.Registry;

namespace VoxelForge.Shared.World;

public class SubChunk
{
    public const int Size = 16;
    private ushort[,,] _blockStateIds;
    private Vector3 _subChunkRelativePosition; // Position relative to the parent chunk (x, y, z offset in chunk units)
    

    public SubChunk(Vector3 subChunkRelativePosition, ushort[,,] blockStates)
    {
        _blockStateIds = blockStates;
        _subChunkRelativePosition = subChunkRelativePosition;
    }

    public void SetBlockStateId(int x, int y, int z, ushort id)
    {
        _blockStateIds[x, y, z] = id;
    }
    
    public BlockState GetBlockState(int x, int y, int z)
        => BlockStateRegistry.GetState(_blockStateIds[x, y, z]);
    
    public ushort GetBlockStateId(int x, int y, int z) => _blockStateIds[x, y, z];

    public void SetBlockState(int x, int y, int z, BlockState state)
        => _blockStateIds[x, y, z] = BlockStateRegistry.GetId(state);
    
    public Vector3 GetWorldPosition(Vector3 chunkWorldPosition)
    {
        return chunkWorldPosition + _subChunkRelativePosition * 16; // Each subchunk is 16x16x16 blocks
    }
    
    public Vector3 GetSubChunkRelativePosition()
    {
        return _subChunkRelativePosition;
    }

    public void Update(World world, Chunk chunk)
    {
        // Placeholder for any per-tick updates needed for the subchunk
    }
}