namespace VoxelForge.Shared.World;

/// <summary>
/// Represents the game world, which is a 3D grid of chunks.
/// The world manages chunk storage, retrieval, and updates.
/// </summary>
public class World
{
    Chunk[,,] Chunks { get; }
    
    /// <summary>
    /// Gets the number of chunks in the X dimension.
    /// </summary>
    public int SizeX => Chunks.GetLength(0);
    
    /// <summary>
    /// Gets the number of chunks in the Y dimension.
    /// </summary>
    public int SizeY => Chunks.GetLength(1);
    
    /// <summary>
    /// Gets the number of chunks in the Z dimension.
    /// </summary>
    public int SizeZ => Chunks.GetLength(2);
    
    /// <summary>
    /// Initializes a new World with the specified dimensions.
    /// </summary>
    /// <param name="sizeX">The number of chunks in the X dimension.</param>
    /// <param name="sizeY">The number of chunks in the Y dimension.</param>
    /// <param name="sizeZ">The number of chunks in the Z dimension.</param>
    public World(int sizeX, int sizeY, int sizeZ)
    {
        Chunks = new Chunk[sizeX, sizeY, sizeZ];
    }
    
    /// <summary>
    /// Gets a chunk at the specified chunk coordinates.
    /// </summary>
    /// <param name="x">The X coordinate of the chunk.</param>
    /// <param name="y">The Y coordinate of the chunk.</param>
    /// <param name="z">The Z coordinate of the chunk.</param>
    /// <returns>The chunk at the specified coordinates.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are out of bounds.</exception>
    public Chunk GetChunk(int x, int y, int z)
    {
        if (x < 0 || x >= Chunks.GetLength(0) ||
            y < 0 || y >= Chunks.GetLength(1) ||
            z < 0 || z >= Chunks.GetLength(2))
        {
            throw new ArgumentOutOfRangeException("Chunk coordinates are out of bounds.");
        }
        return Chunks[x, y, z];
    }
    
    /// <summary>
    /// Sets a chunk at the specified chunk coordinates.
    /// </summary>
    /// <param name="x">The X coordinate of the chunk.</param>
    /// <param name="y">The Y coordinate of the chunk.</param>
    /// <param name="z">The Z coordinate of the chunk.</param>
    /// <param name="chunk">The chunk to place at the specified coordinates.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are out of bounds.</exception>
    public void SetChunk(int x, int y, int z, Chunk chunk)
    {
        if (x < 0 || x >= Chunks.GetLength(0) ||
            y < 0 || y >= Chunks.GetLength(1) ||
            z < 0 || z >= Chunks.GetLength(2))
        {
            throw new ArgumentOutOfRangeException("Chunk coordinates are out of bounds.");
        }
        Chunks[x, y, z] = chunk;
    }
    
    /// <summary>
    /// Gets all non-null chunks in the world.
    /// </summary>
    /// <returns>An enumerable collection of all chunks that have been set.</returns>
    public IEnumerable<Chunk> GetAllChunks()
    {
        foreach (var chunk in Chunks)
        {
            if (chunk != null)
                yield return chunk;
        }
    }
    
    /// <summary>
    /// Updates all chunks in the world. Called once per game tick.
    /// </summary>
    public void Update()
    {
        foreach (var chunk in Chunks)
        {
            chunk?.Update(this);
        }
    }
    
    /// <summary>
    /// Updates a specific chunk. Called when only one chunk needs updating.
    /// </summary>
    /// <param name="x">The X coordinate of the chunk to update.</param>
    /// <param name="y">The Y coordinate of the chunk to update.</param>
    /// <param name="z">The Z coordinate of the chunk to update.</param>
    public void UpdateChunk(int x, int y, int z)
    {
        var chunk = GetChunk(x, y, z);
        chunk?.Update(this);
    }
}