using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using VoxelForge.Client.Rendering;

namespace VoxelForge.Client.UI;

// Simple text renderer using OpenGL for menus and UI
public class TextRenderer : IDisposable
{
    private ShaderProgram _shader;
    private int _vao;
    private int _vbo;
    private int _screenWidth;
    private int _screenHeight;
    
    // Simple 8x8 bitmap font data for ASCII characters 32-126
    private Dictionary<char, float[]> _charData = new Dictionary<char, float[]>();
    
    public TextRenderer(int screenWidth, int screenHeight)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        
        // Create shader for text rendering
        string vertShader = @"
#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoord;

uniform mat4 projection;

void main()
{
    gl_Position = projection * vec4(aPos.xy, 0.0, 1.0);
    TexCoord = aTexCoord;
}";
        
        string fragShader = @"
#version 330 core
in vec2 TexCoord;
out vec4 FragColor;

uniform vec3 textColor;

void main()
{
    // Simple colored rectangle for now
    FragColor = vec4(textColor, 1.0);
}";
        
        _shader = new ShaderProgram(vertShader, fragShader);
        
        // Create VAO and VBO for rendering quads
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsage.DynamicDraw);
        
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        
        InitializeCharData();
    }
    
    public void UpdateScreenSize(int width, int height)
    {
        _screenWidth = width;
        _screenHeight = height;
    }
    
    private void InitializeCharData()
    {
        // For now, render all characters as simple rectangles
        // This is a placeholder until proper font texture atlas is added
        for (char c = ' '; c <= '~'; c++)
        {
            _charData[c] = new float[] { 8, 8 }; // width, height in pixels
        }
    }
    
    public void RenderText(string text, float x, float y, float scale, Vector3 color)
    {
        _shader.Use();
        
        // Create orthographic projection matrix
        var projection = Matrix4.CreateOrthographicOffCenter(0, _screenWidth, _screenHeight, 0, -1, 1);
        _shader.SetUniform("projection", projection);
        _shader.SetUniform("textColor", color);
        
        GL.BindVertexArray(_vao);
        
        float xPos = x;
        foreach (char ch in text)
        {
            char c = ch;
            if (!_charData.ContainsKey(c))
                c = '?';
            
            float charWidth = _charData[c][0] * scale;
            float charHeight = _charData[c][1] * scale;
            
            // Create quad for character
            float[] vertices = {
                // pos       // tex
                xPos, y + charHeight,           0.0f, 1.0f,
                xPos, y,                        0.0f, 0.0f,
                xPos + charWidth, y,            1.0f, 0.0f,
                
                xPos, y + charHeight,           0.0f, 1.0f,
                xPos + charWidth, y,            1.0f, 0.0f,
                xPos + charWidth, y + charHeight, 1.0f, 1.0f
            };
            
            // Update VBO for this character
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            
            // Render quad
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            
            // Advance cursor for next character
            xPos += charWidth + (2 * scale); // spacing between characters
        }
        
        GL.BindVertexArray(0);
    }
    
    public void Dispose()
    {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        _shader.Dispose();
    }
}
