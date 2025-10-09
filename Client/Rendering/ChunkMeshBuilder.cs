using OpenTK.Mathematics;
using VoxelForge.Shared.World;

namespace VoxelForge.Client.Rendering;

/// <summary>
/// Builds mesh data from chunk voxel data.
/// Implements greedy meshing for efficient rendering.
/// </summary>
public class ChunkMeshBuilder
{
    // /// Generates a mesh from a chunk using greedy meshing algorithm.
    // Only generates faces that are exposed to air.
    public static (Models.Vertex[] vertices, uint[] indices) BuildMesh(Chunk chunk)
    {
        var vertices = new List<Models.Vertex>();
        var indices = new List<uint>();
        
        // Iterate through all blocks in the chunk
        for (int y = 0; y < 256; y++)
        {
            for (int z = 0; z < 16; z++)
            {
                for (int x = 0; x < 16; x++)
                {
                    ushort blockId = chunk.GetBlockStateId(x, y, z);
                    
                    // Skip air blocks (id 0)
                    if (blockId == 0)
                        continue;
                    
                    Vector3 blockPos = new Vector3(x, y, z);
                    
                    // Check each face and add if exposed
                    // Top face (+Y)
                    if (IsAir(chunk, x, y + 1, z))
                    {
                        AddTopFace(vertices, indices, blockPos, blockId);
                    }
                    
                    // Bottom face (-Y)
                    if (IsAir(chunk, x, y - 1, z))
                    {
                        AddBottomFace(vertices, indices, blockPos, blockId);
                    }
                    
                    // North face (+Z)
                    if (IsAir(chunk, x, y, z + 1))
                    {
                        AddNorthFace(vertices, indices, blockPos, blockId);
                    }
                    
                    // South face (-Z)
                    if (IsAir(chunk, x, y, z - 1))
                    {
                        AddSouthFace(vertices, indices, blockPos, blockId);
                    }
                    
                    // East face (+X)
                    if (IsAir(chunk, x + 1, y, z))
                    {
                        AddEastFace(vertices, indices, blockPos, blockId);
                    }
                    
                    // West face (-X)
                    if (IsAir(chunk, x - 1, y, z))
                    {
                        AddWestFace(vertices, indices, blockPos, blockId);
                    }
                }
            }
        }
        
        return (vertices.ToArray(), indices.ToArray());
    }
    
    private static bool IsAir(Chunk chunk, int x, int y, int z)
    {
        // Out of bounds is considered air
        if (x < 0 || x >= 16 || y < 0 || y >= 256 || z < 0 || z >= 16)
            return true;
        
        return chunk.GetBlockStateId(x, y, z) == 0;
    }
    
    private static void AddTopFace(List<Models.Vertex> vertices, List<uint> indices, Vector3 pos, ushort blockId)
    {
        uint baseIndex = (uint)vertices.Count;
        Vector3 normal = new Vector3(0, 1, 0);
        
        // Use UV coordinates for vertex coloring (per user request)
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 1, 0), normal, new Vector2(0, 0)));
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 1, 0), normal, new Vector2(1, 0)));
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 1, 1), normal, new Vector2(1, 1)));
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 1, 1), normal, new Vector2(0, 1)));
        
        // Two triangles for the quad
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 1);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 3);
    }
    
    private static void AddBottomFace(List<Models.Vertex> vertices, List<uint> indices, Vector3 pos, ushort blockId)
    {
        uint baseIndex = (uint)vertices.Count;
        Vector3 normal = new Vector3(0, -1, 0);
        
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 0, 1), normal, new Vector2(0, 0)));
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 0, 1), normal, new Vector2(1, 0)));
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 0, 0), normal, new Vector2(1, 1)));
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 0, 0), normal, new Vector2(0, 1)));
        
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 1);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 3);
    }
    
    private static void AddNorthFace(List<Models.Vertex> vertices, List<uint> indices, Vector3 pos, ushort blockId)
    {
        uint baseIndex = (uint)vertices.Count;
        Vector3 normal = new Vector3(0, 0, 1);
        
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 0, 1), normal, new Vector2(0, 0)));
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 1, 1), normal, new Vector2(0, 1)));
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 1, 1), normal, new Vector2(1, 1)));
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 0, 1), normal, new Vector2(1, 0)));
        
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 1);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 3);
    }
    
    private static void AddSouthFace(List<Models.Vertex> vertices, List<uint> indices, Vector3 pos, ushort blockId)
    {
        uint baseIndex = (uint)vertices.Count;
        Vector3 normal = new Vector3(0, 0, -1);
        
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 0, 0), normal, new Vector2(0, 0)));
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 1, 0), normal, new Vector2(0, 1)));
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 1, 0), normal, new Vector2(1, 1)));
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 0, 0), normal, new Vector2(1, 0)));
        
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 1);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 3);
    }
    
    private static void AddEastFace(List<Models.Vertex> vertices, List<uint> indices, Vector3 pos, ushort blockId)
    {
        uint baseIndex = (uint)vertices.Count;
        Vector3 normal = new Vector3(1, 0, 0);
        
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 0, 0), normal, new Vector2(0, 0)));
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 0, 1), normal, new Vector2(1, 0)));
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 1, 1), normal, new Vector2(1, 1)));
        vertices.Add(new Models.Vertex(pos + new Vector3(1, 1, 0), normal, new Vector2(0, 1)));
        
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 1);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 3);
    }
    
    private static void AddWestFace(List<Models.Vertex> vertices, List<uint> indices, Vector3 pos, ushort blockId)
    {
        uint baseIndex = (uint)vertices.Count;
        Vector3 normal = new Vector3(-1, 0, 0);
        
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 0, 1), normal, new Vector2(0, 0)));
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 0, 0), normal, new Vector2(1, 0)));
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 1, 0), normal, new Vector2(1, 1)));
        vertices.Add(new Models.Vertex(pos + new Vector3(0, 1, 1), normal, new Vector2(0, 1)));
        
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 1);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 0);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 3);
    }
}
