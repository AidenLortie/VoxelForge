using System.Numerics;
using VoxelForge.Shared.Content.Blocks;

namespace VoxelForge.Shared.World;

public class Chunk
{
    SubChunk[] SubChunks = new SubChunk[16]; // 16 * 16 = 256 blocks in height
    Vector2 ChunkPosition; // Position in chunk coordinates (not block coordinates)
    private World World;
    
    public Chunk(Vector2 chunkPosition, World parentWorld)
    {
        ChunkPosition = chunkPosition;
        World = parentWorld;
        // Initialize subchunks
        for (int i = 0; i < 16; i++)
        {
            SubChunks[i] = new SubChunk(new Vector3(0, i, 0), new BlockState[16,16,16]); // Each subchunk is 16 blocks high
        }
    }
    
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
    
    public Vector2 GetChunkPosition()
    {
        return ChunkPosition;
    }
    
    public void Update()
    {
        foreach (var subChunk in SubChunks)
        {
            subChunk.Update();
        }
    }
    
    public Vector3 GetWorldPosition()
    {
        return new Vector3(ChunkPosition.X * 16, 0, ChunkPosition.Y * 16); // Each chunk is 16x16 blocks in XZ plane
    }

    
}