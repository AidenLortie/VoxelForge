namespace VoxelForge.Shared.World;

public class World
{
    Chunk[,,] Chunks { get; }
    public World(int sizeX, int sizeY, int sizeZ)
    {
        Chunks = new Chunk[sizeX, sizeY, sizeZ];
    }
    
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
    
    public IEnumerable<Chunk> GetAllChunks()
    {
        foreach (var chunk in Chunks)
        {
            if (chunk != null)
                yield return chunk;
        }
    }
    
    public void Update()
    {
        foreach (var chunk in Chunks)
        {
            chunk?.Update(this);
        }
    }
    
    public void UpdateChunk(int x, int y, int z)
    {
        var chunk = GetChunk(x, y, z);
        chunk?.Update(this);
    }
}