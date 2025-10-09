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
    private readonly Texture _texture;
    private bool _disposed = false;
    
    // Block colors for different block types (using block state IDs)
    private readonly Dictionary<ushort, Vector3> _blockColors = new()
    {
        { 0, new Vector3(1.0f, 1.0f, 1.0f) }, // Air (white, shouldn't be rendered)
        { 1, new Vector3(0.5f, 0.5f, 0.5f) }, // Stone (gray)
        { 2, new Vector3(0.4f, 0.8f, 0.3f) }, // Grass (green)
        { 3, new Vector3(0.6f, 0.4f, 0.2f) }, // Dirt (brown)
    };
    
    // /// Creates a new ChunkRenderer with the specified shader and texture.
    public ChunkRenderer(ShaderProgram shader, Texture texture)
    {
        _shader = shader;
        _texture = texture;
    }
    
    // /// Updates the mesh for a chunk. If the chunk already has a mesh, it will be replaced.
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
    
    // /// Removes a chunk's mesh from the renderer.
    public void RemoveChunk(Vector2i chunkPos)
    {
        if (_chunkMeshes.TryGetValue(chunkPos, out var mesh))
        {
            mesh.Dispose();
            _chunkMeshes.Remove(chunkPos);
        }
    }
    
    // /// Renders all loaded chunks with the given view and projection matrices.
    public void Render(Matrix4 view, Matrix4 projection)
    {
        _shader.Use();
        _shader.SetUniform("view", view);
        _shader.SetUniform("projection", projection);
        
        // Bind texture
        _texture.Bind(0);
        _shader.SetUniform("blockTexture", 0);
        
        // Default block color (white for stone)
        _shader.SetUniform("blockColor", _blockColors.GetValueOrDefault((ushort)1, Vector3.One));
        
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
    
    // /// Gets the number of chunks currently loaded.
    public int GetChunkCount()
    {
        return _chunkMeshes.Count;
    }
    
    // /// Disposes all chunk meshes and frees GPU resources.
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
