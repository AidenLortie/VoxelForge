using OpenTK.Mathematics;
using VoxelForge.Shared.World;

namespace VoxelForge.Client.Rendering;

/// <summary>
/// Manages rendering of chunks.
/// Maintains a cache of chunk meshes and renders them efficiently.
/// </summary>
public class ChunkRenderer : IDisposable
{
    private readonly Dictionary<Vector2i, Models.Model> _chunkMeshes = new();
    private readonly ShaderProgram _shader;
    private bool _disposed = false;
    
    /// <summary>
    /// Creates a new ChunkRenderer with the specified shader.
    /// </summary>
    public ChunkRenderer(ShaderProgram shader)
    {
        _shader = shader;
    }
    
    /// <summary>
    /// Updates the mesh for a chunk. If the chunk already has a mesh, it will be replaced.
    /// </summary>
    /// <param name="chunk">The chunk to update</param>
    public void UpdateChunk(Chunk chunk)
    {
        var chunkPos = new Vector2i((int)chunk.GetChunkPosition().X, (int)chunk.GetChunkPosition().Y);
        
        // Remove old mesh if it exists
        if (_chunkMeshes.TryGetValue(chunkPos, out var oldMesh))
        {
            oldMesh.Dispose();
            _chunkMeshes.Remove(chunkPos);
        }
        
        // Build new mesh
        var (vertices, indices) = ChunkMeshBuilder.BuildMesh(chunk);
        
        // Only create model if there are vertices
        if (vertices.Length > 0)
        {
            var model = new Models.Model($"chunk_{chunkPos.X}_{chunkPos.Y}", vertices, indices);
            _chunkMeshes[chunkPos] = model;
            
            Console.WriteLine($"Chunk ({chunkPos.X}, {chunkPos.Y}) mesh updated: {vertices.Length} vertices, {indices.Length} indices");
        }
    }
    
    /// <summary>
    /// Removes a chunk's mesh from the renderer.
    /// </summary>
    public void RemoveChunk(Vector2i chunkPos)
    {
        if (_chunkMeshes.TryGetValue(chunkPos, out var mesh))
        {
            mesh.Dispose();
            _chunkMeshes.Remove(chunkPos);
        }
    }
    
    /// <summary>
    /// Renders all loaded chunks with the given view and projection matrices.
    /// </summary>
    /// <param name="view">View matrix (camera transform)</param>
    /// <param name="projection">Projection matrix (perspective)</param>
    public void Render(Matrix4 view, Matrix4 projection)
    {
        _shader.Use();
        _shader.SetUniform("view", view);
        _shader.SetUniform("projection", projection);
        
        foreach (var kvp in _chunkMeshes)
        {
            var chunkPos = kvp.Key;
            var model = kvp.Value;
            
            // Calculate model matrix (chunk position in world)
            var modelMatrix = Matrix4.CreateTranslation(chunkPos.X * 16, 0, chunkPos.Y * 16);
            _shader.SetUniform("model", modelMatrix);
            
            model.Render();
        }
    }
    
    /// <summary>
    /// Gets the number of chunks currently loaded.
    /// </summary>
    public int GetChunkCount()
    {
        return _chunkMeshes.Count;
    }
    
    /// <summary>
    /// Disposes all chunk meshes and frees GPU resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (var mesh in _chunkMeshes.Values)
            {
                mesh.Dispose();
            }
            _chunkMeshes.Clear();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
