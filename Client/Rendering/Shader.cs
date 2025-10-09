using OpenTK.Graphics.OpenGL;

namespace VoxelForge.Client.Rendering;

/// <summary>
/// Represents a single shader (vertex, fragment, geometry, etc.).
/// </summary>
public class Shader : IDisposable
{
    private int _handle;
    private readonly ShaderType _type;
    private bool _disposed = false;
    
    // /// Gets the OpenGL handle for this shader.
    public int Handle => _handle;
    
    // /// Gets the type of this shader.
    public ShaderType Type => _type;
    
    // /// Creates a shader from source code.
    public Shader(string source, ShaderType type)
    {
        _type = type;
        _handle = GL.CreateShader(type);
        
        // Upload source code and compile
        GL.ShaderSource(_handle, source);
        GL.CompileShader(_handle);
        
        // Check for compilation errors using unsafe code
        unsafe
        {
            int success;
            GL.GetShaderiv(_handle, ShaderParameterName.CompileStatus, &success);
            if (success == 0)
            {
                GL.GetShaderInfoLog(_handle, 1024, out int length, out string infoLog);
                throw new Exception($"Shader compilation failed ({type}):\n{infoLog}");
            }
        }
        
        Console.WriteLine($"Shader compiled successfully ({type})");
    }
    
    // /// Disposes the shader and frees GPU resources.
    public void Dispose()
    {
        if (!_disposed)
        {
            GL.DeleteShader(_handle);
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
    
    ~Shader()
    {
        if (!_disposed)
        {
            Console.WriteLine($"Warning: Shader ({_type}) was not disposed properly");
        }
    }
}
