using System.Numerics;
using VoxelForge.Shared.Content.Blocks;

namespace VoxelForge.Shared.World;

public class SubChunk
{
    private BlockState[,,] _blockstates;
    private Vector3 _subChunkRelativePosition; // Position relative to the parent chunk (x, y, z offset in chunk units)

    public SubChunk(Vector3 subChunkRelativePosition, BlockState[,,] blockStates)
    {
        _blockstates = blockStates;
        _subChunkRelativePosition = subChunkRelativePosition;
    }
    
    public BlockState GetBlockState(int x, int y, int z) // x, y, z are local coordinates within the subchunk (0-15)
    {
        if (x < 0 || x >= 16 || y < 0 || y >= 16 || z < 0 || z >= 16)
            throw new ArgumentOutOfRangeException("Block coordinates must be between 0 and 15.");
        return _blockstates[x, y, z];
    }
    
    public void SetBlockState(int x, int y, int z, BlockState state) // x, y, z are local coordinates within the subchunk (0-15)
    {
        if (x < 0 || x >= 16 || y < 0 || y >= 16 || z < 0 || z >= 16)
            throw new ArgumentOutOfRangeException("Block coordinates must be between 0 and 15.");
        _blockstates[x, y, z] = state;
    }
    
    public Vector3 GetWorldPosition(Vector3 chunkWorldPosition)
    {
        return chunkWorldPosition + _subChunkRelativePosition * 16; // Each subchunk is 16x16x16 blocks
    }
    
    public Vector3 GetSubChunkRelativePosition()
    {
        return _subChunkRelativePosition;
    }
    
    public void Update()
    {
        // Placeholder for any per-tick updates needed for the subchunk
    }
}