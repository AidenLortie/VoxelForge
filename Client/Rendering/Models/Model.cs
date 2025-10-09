using OpenTK.Graphics.OpenGL;

namespace VoxelForge.Client.Rendering.Models;

/// <summary>
/// Represents a renderable 3D model with GPU buffers.
/// Manages VAO, VBO, and EBO for efficient rendering.
/// </summary>
public class Model : IDisposable
{
    private int _vao;
    private int _vbo;
    private int _ebo;
    private int _indexCount;
    private bool _disposed = false;
    
    /// <summary>
    /// Name/ID of the model
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Gets the number of indices to render
    /// </summary>
    public int IndexCount => _indexCount;
    
    /// <summary>
    /// Creates a model from vertex and index data.
    /// </summary>
    /// <param name="name">Name/ID of the model</param>
    /// <param name="vertices">Array of interleaved vertex data</param>
    /// <param name="indices">Array of indices for indexed drawing</param>
    public Model(string name, Vertex[] vertices, uint[] indices)
    {
        Name = name;
        _indexCount = indices.Length;
        
        // Generate VAO
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        
        // Generate and upload VBO
        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        
        // Convert vertex data to byte array
        float[] vertexData = new float[vertices.Length * 8]; // 8 floats per vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            int offset = i * 8;
            vertexData[offset + 0] = vertices[i].Position.X;
            vertexData[offset + 1] = vertices[i].Position.Y;
            vertexData[offset + 2] = vertices[i].Position.Z;
            vertexData[offset + 3] = vertices[i].Normal.X;
            vertexData[offset + 4] = vertices[i].Normal.Y;
            vertexData[offset + 5] = vertices[i].Normal.Z;
            vertexData[offset + 6] = vertices[i].TexCoord.X;
            vertexData[offset + 7] = vertices[i].TexCoord.Y;
        }
        
        GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), 
                     vertexData, BufferUsage.StaticDraw);
        
        // Set up vertex attributes
        // Position (location = 0)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 
                              Vertex.SizeInBytes, Vertex.PositionOffset);
        GL.EnableVertexAttribArray(0);
        
        // Normal (location = 1)
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 
                              Vertex.SizeInBytes, Vertex.NormalOffset);
        GL.EnableVertexAttribArray(1);
        
        // TexCoord (location = 2)
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 
                              Vertex.SizeInBytes, Vertex.TexCoordOffset);
        GL.EnableVertexAttribArray(2);
        
        // Generate and upload EBO
        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), 
                     indices, BufferUsage.StaticDraw);
        
        // Unbind
        GL.BindVertexArray(0);
        
        Console.WriteLine($"Model '{name}' created: {vertices.Length} vertices, {indices.Length} indices");
    }
    
    /// <summary>
    /// Creates a model from a JSON model definition.
    /// </summary>
    public static Model FromDefinition(ModelDefinition definition)
    {
        var vertices = definition.ToVertices();
        return new Model(definition.Name, vertices, definition.Indices);
    }
    
    /// <summary>
    /// Binds this model's VAO for rendering.
    /// </summary>
    public void Bind()
    {
        GL.BindVertexArray(_vao);
    }
    
    /// <summary>
    /// Renders this model using indexed drawing.
    /// </summary>
    public void Render()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
    }
    
    /// <summary>
    /// Disposes the model and frees GPU resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
    
    ~Model()
    {
        if (!_disposed)
        {
            Console.WriteLine($"Warning: Model '{Name}' was not disposed properly");
        }
    }
}
