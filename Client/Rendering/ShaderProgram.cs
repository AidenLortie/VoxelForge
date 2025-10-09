using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace VoxelForge.Client.Rendering;

/// <summary>
/// Represents a complete shader program with vertex and fragment shaders.
/// Manages shader compilation, linking, and uniform setting.
/// </summary>
public class ShaderProgram : IDisposable
{
    private int _handle;
    private bool _disposed = false;
    private readonly Dictionary<string, int> _uniformLocations = new();
    
    // /// Gets the OpenGL handle for this shader program.
    public int Handle => _handle;
    
    // /// Creates a shader program from vertex and fragment shader source code.
    public ShaderProgram(string vertexSource, string fragmentSource)
    {
        // Create shaders
        using var vertexShader = new Shader(vertexSource, ShaderType.VertexShader);
        using var fragmentShader = new Shader(fragmentSource, ShaderType.FragmentShader);
        
        // Create program
        _handle = GL.CreateProgram();
        
        // Attach shaders
        GL.AttachShader(_handle, vertexShader.Handle);
        GL.AttachShader(_handle, fragmentShader.Handle);
        
        // Link program
        GL.LinkProgram(_handle);
        
        // Check for linking errors using unsafe code
        unsafe
        {
            int success;
            GL.GetProgramiv(_handle, ProgramProperty.LinkStatus, &success);
            if (success == 0)
            {
                GL.GetProgramInfoLog(_handle, 1024, out int length, out string infoLog);
                throw new Exception($"Shader program linking failed:\n{infoLog}");
            }
        }
        
        // Detach shaders after linking (they're no longer needed)
        GL.DetachShader(_handle, vertexShader.Handle);
        GL.DetachShader(_handle, fragmentShader.Handle);
        
        Console.WriteLine("Shader program linked successfully");
        
        // Cache uniform locations
        CacheUniformLocations();
    }
    
    // /// Caches all uniform locations for faster access.
    private void CacheUniformLocations()
    {
        unsafe
        {
            int uniformCount;
            GL.GetProgramiv(_handle, ProgramProperty.ActiveUniforms, &uniformCount);
            
            for (uint i = 0; i < uniformCount; i++)
            {
                int length, size;
                UniformType type;
                byte* namePtr = stackalloc byte[256];
                GL.GetActiveUniform(_handle, i, 256, &length, &size, &type, namePtr);
                string name = System.Text.Encoding.UTF8.GetString(namePtr, length);
                int location = GL.GetUniformLocation(_handle, name);
                _uniformLocations[name] = location;
            }
            
            Console.WriteLine($"Cached {uniformCount} uniform locations");
        }
    }
    
    // /// Activates this shader program for rendering.
    public void Use()
    {
        GL.UseProgram(_handle);
    }
    
    // /// Gets the location of a uniform variable.
    public int GetUniformLocation(string name)
    {
        if (_uniformLocations.TryGetValue(name, out int location))
            return location;
        
        // Not cached, try to get it
        location = GL.GetUniformLocation(_handle, name);
        _uniformLocations[name] = location;
        
        if (location == -1)
            Console.WriteLine($"Warning: Uniform '{name}' not found in shader program");
        
        return location;
    }
    
    // /// Sets a float uniform value.
    public void SetUniform(string name, float value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
            GL.Uniform1f(location, value);
    }
    
    // /// Sets an integer uniform value.
    public void SetUniform(string name, int value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
            GL.Uniform1i(location, value);
    }
    
    // /// Sets a Vector3 uniform value.
    public void SetUniform(string name, Vector3 value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
            GL.Uniform3f(location, value.X, value.Y, value.Z);
    }
    
    // /// Sets a Vector4 uniform value.
    public void SetUniform(string name, Vector4 value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
            GL.Uniform4f(location, value.X, value.Y, value.Z, value.W);
    }
    
    // /// Sets a Matrix4 uniform value.
    public void SetUniform(string name, Matrix4 value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
        {
            unsafe
            {
                GL.UniformMatrix4fv(location, 1, false, (float*)&value);
            }
        }
    }
    
    // /// Disposes the shader program and frees GPU resources.
    public void Dispose()
    {
        if (!_disposed)
        {
            GL.DeleteProgram(_handle);
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
    
    ~ShaderProgram()
    {
        if (!_disposed)
        {
            Console.WriteLine("Warning: ShaderProgram was not disposed properly");
        }
    }
}
