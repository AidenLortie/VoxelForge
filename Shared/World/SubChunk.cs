using System.Numerics;
using VoxelForge.Shared.Content.Blocks;
using VoxelForge.Shared.Registry;

namespace VoxelForge.Shared.World;

/// <summary>
/// Represents a 16×16×16 block section within a Chunk.
/// SubChunks are the smallest unit of world storage and are stacked vertically to form a complete Chunk.
/// </summary>
public class SubChunk
{
    /// <summary>
    /// The size of a SubChunk in each dimension (16 blocks).
    /// </summary>
    public const int Size = 16;
    
    private ushort[,,] _blockStateIds;
    private Vector3 _subChunkRelativePosition; // Position relative to the parent chunk (x, y, z offset in chunk units)
    

    /// <summary>
    /// Initializes a new SubChunk with the specified position and block state data.
    /// </summary>
    /// <param name="subChunkRelativePosition">Position relative to parent chunk (in subchunk units).</param>
    /// <param name="blockStates">3D array of block state IDs (16×16×16).</param>
    public SubChunk(Vector3 subChunkRelativePosition, ushort[,,] blockStates)
    {
        _blockStateIds = blockStates;
        _subChunkRelativePosition = subChunkRelativePosition;
    }

    /// <summary>
    /// Sets the block state ID at the specified local coordinates.
    /// </summary>
    /// <param name="x">Local X coordinate (0-15).</param>
    /// <param name="y">Local Y coordinate (0-15).</param>
    /// <param name="z">Local Z coordinate (0-15).</param>
    /// <param name="id">The block state ID to set.</param>
    public void SetBlockStateId(int x, int y, int z, ushort id)
    {
        _blockStateIds[x, y, z] = id;
    }
    
    /// <summary>
    /// Gets the block state at the specified local coordinates.
    /// </summary>
    /// <param name="x">Local X coordinate (0-15).</param>
    /// <param name="y">Local Y coordinate (0-15).</param>
    /// <param name="z">Local Z coordinate (0-15).</param>
    /// <returns>The BlockState at the specified position.</returns>
    public BlockState GetBlockState(int x, int y, int z)
        => BlockStateRegistry.GetState(_blockStateIds[x, y, z]);
    
    /// <summary>
    /// Gets the block state ID at the specified local coordinates.
    /// </summary>
    /// <param name="x">Local X coordinate (0-15).</param>
    /// <param name="y">Local Y coordinate (0-15).</param>
    /// <param name="z">Local Z coordinate (0-15).</param>
    /// <returns>The block state ID at the specified position.</returns>
    public ushort GetBlockStateId(int x, int y, int z) => _blockStateIds[x, y, z];

    /// <summary>
    /// Sets the block state at the specified local coordinates.
    /// </summary>
    /// <param name="x">Local X coordinate (0-15).</param>
    /// <param name="y">Local Y coordinate (0-15).</param>
    /// <param name="z">Local Z coordinate (0-15).</param>
    /// <param name="state">The BlockState to set.</param>
    public void SetBlockState(int x, int y, int z, BlockState state)
        => _blockStateIds[x, y, z] = BlockStateRegistry.GetId(state);
    
    /// <summary>
    /// Gets the world position of this SubChunk in block coordinates.
    /// </summary>
    /// <param name="chunkWorldPosition">The world position of the parent Chunk.</param>
    /// <returns>The world position of this SubChunk's origin.</returns>
    public Vector3 GetWorldPosition(Vector3 chunkWorldPosition)
    {
        return chunkWorldPosition + _subChunkRelativePosition * 16; // Each subchunk is 16x16x16 blocks
    }
    
    /// <summary>
    /// Gets the position of this SubChunk relative to its parent Chunk.
    /// </summary>
    /// <returns>The relative position in subchunk units.</returns>
    public Vector3 GetSubChunkRelativePosition()
    {
        return _subChunkRelativePosition;
    }

    /// <summary>
    /// Updates this SubChunk. Called once per game tick.
    /// </summary>
    /// <param name="world">The World this SubChunk belongs to.</param>
    /// <param name="chunk">The parent Chunk this SubChunk belongs to.</param>
    public void Update(World world, Chunk chunk)
    {
        // Placeholder for any per-tick updates needed for the subchunk
    }
}