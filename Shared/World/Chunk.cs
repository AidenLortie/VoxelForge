using System.Numerics;
using VoxelForge.Shared.Content.Blocks;

namespace VoxelForge.Shared.World;

/// <summary>
/// Represents a 16×256×16 block region of the world, divided into 16 SubChunks vertically.
/// Chunks are the primary unit for world storage and network transmission.
/// </summary>
public class Chunk
{
    /// <summary>
    /// Array of 16 SubChunks, each 16×16×16 blocks, stacked vertically to form a 256-block tall column.
    /// </summary>
    public readonly SubChunk[] SubChunks = new SubChunk[16]; // 16 * 16 = 256 blocks in height
    
    Vector2 ChunkPosition; // Position in chunk coordinates (not block coordinates)
    
    /// <summary>
    /// Initializes a new Chunk at the specified chunk position.
    /// </summary>
    /// <param name="chunkPosition">The position of this chunk in chunk coordinates (X, Z).</param>
    public Chunk(Vector2 chunkPosition)
    {
        ChunkPosition = chunkPosition;
        // Initialize subchunks
        for (int i = 0; i < 16; i++)
        {
            SubChunks[i] = new SubChunk(new Vector3(0, i, 0), new ushort[16,16,16]); // Each subchunk is 16 blocks high
        }
    }

    /// <summary>
    /// Initializes a new Chunk with default values. Required for deserialization.
    /// </summary>
    public Chunk() { }
    
    /// <summary>
    /// Gets the block state at the specified local coordinates within this chunk.
    /// </summary>
    /// <param name="x">Local X coordinate (0-15).</param>
    /// <param name="y">Local Y coordinate (0-255).</param>
    /// <param name="z">Local Z coordinate (0-15).</param>
    /// <returns>The BlockState at the specified position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when Y is not between 0 and 255.</exception>
    public BlockState GetBlockState(int x, int y, int z) // x, y, z are global block coordinates
    {
        if (y < 0 || y >= 256)
            throw new ArgumentOutOfRangeException("Y coordinate must be between 0 and 255.");
        
        // Calculate local subchunk coordinates
        int subChunkIndex = y / 16;
        int localY = y % 16;
        int localX = x % 16;
        int localZ = z % 16;
        return SubChunks[subChunkIndex].GetBlockState(localX, localY, localZ);
    }
    
    /// <summary>
    /// Sets the block state at the specified local coordinates within this chunk.
    /// </summary>
    /// <param name="x">Local X coordinate (0-15).</param>
    /// <param name="y">Local Y coordinate (0-255).</param>
    /// <param name="z">Local Z coordinate (0-15).</param>
    /// <param name="state">The BlockState to set at the specified position.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when Y is not between 0 and 255.</exception>
    public void SetBlockState(int x, int y, int z, BlockState state) // x, y, z are global block coordinates
    {
        if (y < 0 || y >= 256)
            throw new ArgumentOutOfRangeException("Y coordinate must be between 0 and 255.");
        
        // Calculate local subchunk coordinates
        int subChunkIndex = y / 16;
        int localY = y % 16;
        int localX = x % 16;
        int localZ = z % 16;
        SubChunks[subChunkIndex].SetBlockState(localX, localY, localZ, state);
    }

    /// <summary>
    /// Sets the block state ID at the specified local coordinates within this chunk.
    /// </summary>
    /// <param name="x">Local X coordinate (0-15).</param>
    /// <param name="y">Local Y coordinate (0-255).</param>
    /// <param name="z">Local Z coordinate (0-15).</param>
    /// <param name="blockStateId">The numeric ID of the block state to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when Y is not between 0 and 255.</exception>
    public void SetBlockStateId(int x, int y, int z, ushort blockStateId)
    {
        if (y < 0 || y >= 256)
            throw new ArgumentOutOfRangeException("Y coordinate must be between 0 and 255.");
        
        int subChunkIndex = y / 16;
        int localY = y % 16;
        int localX = x % 16;
        int localZ = z % 16;
        SubChunks[subChunkIndex].SetBlockStateId(localX, localY, localZ, blockStateId);
    }

    /// <summary>
    /// Gets the block state ID at the specified local coordinates within this chunk.
    /// </summary>
    /// <param name="x">Local X coordinate (0-15).</param>
    /// <param name="y">Local Y coordinate (0-255).</param>
    /// <param name="z">Local Z coordinate (0-15).</param>
    /// <returns>The numeric ID of the block state at the specified coordinates.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when Y is not between 0 and 255.</exception>
    public ushort GetBlockStateId(int x, int y, int z)
    {
        if (y < 0 || y >= 256)
            throw new ArgumentOutOfRangeException("Y coordinate must be between 0 and 255.");
        
        int subChunkIndex = y / 16;
        int localY = y % 16;
        int localX = x % 16;
        int localZ = z % 16;
        return SubChunks[subChunkIndex].GetBlockStateId(localX, localY, localZ);
    }

    /// <summary>
    /// Gets the SubChunk containing the specified coordinates.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate (0-255).</param>
    /// <param name="z">Z coordinate.</param>
    /// <returns>The SubChunk at the calculated index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when Y is not between 0 and 255.</exception>
    public SubChunk GetOrCreateSubChunk(int x, int y, int z)
    {
        if (y < 0 || y >= 256)
            throw new ArgumentOutOfRangeException("Y coordinate must be between 0 and 255.");
        int subChunkIndex = y / 16;
        return SubChunks[subChunkIndex];
    }
    
    /// <summary>
    /// Gets the position of this chunk in chunk coordinates.
    /// </summary>
    /// <returns>A Vector2 representing the chunk position (X, Z).</returns>
    public Vector2 GetChunkPosition()
    {
        return ChunkPosition;
    }
    
    /// <summary>
    /// Updates all SubChunks within this chunk. Called once per game tick.
    /// </summary>
    /// <param name="world">The World this chunk belongs to.</param>
    public void Update(World world)
    {
        foreach (var subChunk in SubChunks)
        {
            subChunk.Update(world, this);
        }
    }
    
    /// <summary>
    /// Gets the world position of this chunk in block coordinates.
    /// </summary>
    /// <returns>A Vector3 representing the world position of the chunk's origin.</returns>
    public Vector3 GetWorldPosition()
    {
        return new Vector3(ChunkPosition.X * 16, 0, ChunkPosition.Y * 16); // Each chunk is 16x16 blocks in XZ plane
    }

    
}