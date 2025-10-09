using System.Numerics;
using VoxelForge.Shared.World;

namespace VoxelForge.Server;

/// <summary>
/// Simple world generator for creating terrain.
/// </summary>
public class WorldGenerator
{
    private readonly Random _random;
    
    public WorldGenerator(int seed = 0)
    {
        _random = seed == 0 ? new Random() : new Random(seed);
    }
    
    /// <summary>
    /// Generates a chunk with simple terrain.
    /// </summary>
    /// <param name="chunkX">Chunk X coordinate</param>
    /// <param name="chunkZ">Chunk Z coordinate</param>
    /// <returns>Generated chunk</returns>
    public Chunk GenerateChunk(int chunkX, int chunkZ)
    {
        var chunk = new Chunk(new Vector2(chunkX, chunkZ));
        
        // Simple terrain generation
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                // Calculate world position
                int worldX = chunkX * 16 + x;
                int worldZ = chunkZ * 16 + z;
                
                // Simple height map using sine waves for variation
                int baseHeight = 32;
                int heightVariation = (int)(Math.Sin(worldX * 0.1) * 4 + Math.Cos(worldZ * 0.1) * 4);
                int terrainHeight = baseHeight + heightVariation;
                
                // Clamp height
                terrainHeight = Math.Clamp(terrainHeight, 1, 60);
                
                // Fill blocks
                for (int y = 0; y < 256; y++)
                {
                    if (y == 0)
                    {
                        // Bedrock layer
                        chunk.SetBlockStateId(x, y, z, 1); // Stone
                    }
                    else if (y < terrainHeight - 3)
                    {
                        // Stone layer
                        chunk.SetBlockStateId(x, y, z, 1); // Stone
                    }
                    else if (y < terrainHeight)
                    {
                        // Dirt layer
                        chunk.SetBlockStateId(x, y, z, 3); // Dirt
                    }
                    else if (y == terrainHeight)
                    {
                        // Grass top layer
                        chunk.SetBlockStateId(x, y, z, 2); // Grass
                    }
                    else
                    {
                        // Air
                        chunk.SetBlockStateId(x, y, z, 0); // Air
                    }
                }
            }
        }
        
        return chunk;
    }
}
