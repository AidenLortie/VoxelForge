using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace VoxelForge.Client.Rendering;

public class ShaderProgram : IDisposable
{
    private string? _vertexShader;
    private string? _fragmentShader;
    
    public int Id { get; }
    
    public ShaderProgram(string vertexShaderSource, string fragmentShaderSource)
    {
        
        _vertexShader = File.ReadAllText(vertexShaderSource);
        _fragmentShader = File.ReadAllText(fragmentShaderSource);
        
        int vertexShaderId = CompileShader(_vertexShader!, ShaderType.VertexShader);
        int fragmentShaderId = CompileShader(_fragmentShader!, ShaderType.FragmentShader);
        
        Id = GL.CreateProgram();
        GL.AttachShader(Id, vertexShaderId);
        GL.AttachShader(Id, fragmentShaderId);
        GL.LinkProgram(Id);
        
        GL.DeleteShader(vertexShaderId);
        GL.DeleteShader(fragmentShaderId);
    }
    
    private int CompileShader(string shaderSource, ShaderType shaderType)
    {
        int shaderId = GL.CreateShader(shaderType);
        GL.ShaderSource(shaderId, shaderSource);
        GL.CompileShader(shaderId);
        
        return shaderId;
    }
    
    public void Use()
    {
        GL.UseProgram(Id);
    }
    
    public void Dispose()
    {
        GL.DeleteProgram(Id);
    }
    
    // UNIFORM SETTERS AND GETTERS
    public void SetUniform(string name, int value)
    {
        int location = GL.GetUniformLocation(Id, name);
        GL.Uniform1i(location, value);
    }
    
    public void SetUniform(string name, float value)
    {
        int location = GL.GetUniformLocation(Id, name);
        GL.Uniform1f(location, value);
    }

    public void SetUniform(string name, Matrix4d value)
    {
        int location = GL.GetUniformLocation(Id, name);
        GL.UniformMatrix4d(location, 1, false, ref value);
    }

    public void SetUniform(string name, Vector2d value)
    {
        int location = GL.GetUniformLocation(Id, name);
        GL.Uniform2d(location, value.X, value.Y);
    }


}