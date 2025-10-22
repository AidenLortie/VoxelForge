using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform;

namespace VoxelForge.Client.Rendering;

public class FormatStringSection
{
    public string Text { get; set; }
    public int Size { get; set; }
    public Vector3 ForegroundColor { get; set; }
    public Vector3 BackgroundColor { get; set; }
    public Matrix4 Transform { get; set; }
    
    
    
    public FormatStringSection()
    {
        Text = string.Empty;
        Size = 12;
        ForegroundColor = Vector3.One;
        BackgroundColor = Vector3.Zero;
        Transform = Matrix4.Identity;
    }
    public FormatStringSection(string text, int size, Vector3 foregroundColor, Vector3 backgroundColor, Matrix4 transform)
    {
        Text = text;
        Size = size;
        ForegroundColor = foregroundColor;
        BackgroundColor = backgroundColor;
        Transform = transform;
    }
}

public class FormatString
{
    public List<FormatStringSection> Sections { get; set; }
    
    public FormatString()
    {
        Sections = new List<FormatStringSection>();
    }
    
    public void AddSection(FormatStringSection section)
    {
        Sections.Add(section);
    }
}



public static class TextRenderer
{
    private static readonly Texture _fontTexture = new Texture("./Assets/Fonts/Miracode.png");
    private static readonly Dictionary<char, Vector2> _charPositions = new(); // top left corner of character in texture
    
    private static readonly ShaderProgram _shaderProgram = new ShaderProgram("./Assets/Shaders/Text.vert", "./Assets/Shaders/Text.frag");
    private static int _vao;
    private static int _vbo;
    private static int _ebo;
    private static readonly List<float> _vertexData = new();
    private static readonly List<int> _indices = new();
    private static int _screenWidth = 1280;
    private static int _screenHeight = 720;
    
    public static void Init()
    {
        int textureWidth = _fontTexture.Width;
        int textureHeight = _fontTexture.Height;

        int charSize = textureWidth / 16;
        for (char c = ' '; c <= '~'; c++)
        {
            int Id = c - ' ';
            _charPositions.Add(c, new Vector2(Id % 16 * charSize, Id / 16 * charSize));
        }
        
        // OpenGL Initialization
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();
        
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        
        // Position attribute (location = 0)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        // Texture coordinate attribute (location = 1)
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 11 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        
        // Foreground color attribute (location = 2)
        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 5 * sizeof(float));
        GL.EnableVertexAttribArray(2);
        
        // Background color attribute (location = 3)
        GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 8 * sizeof(float));
        GL.EnableVertexAttribArray(3);
        
        GL.BindVertexArray(0);
    }
    
    public static void SetScreenSize(int width, int height)
    {
        _screenWidth = width;
        _screenHeight = height;
    }
    
    
    
    public static void Render(FormatString formatString, float xPos = 10, float yPos = 10)
    {
        _shaderProgram.Use();
        _fontTexture.Bind();
        _vertexData.Clear();
        _indices.Clear();
        
        float xOffset = xPos;
        float yOffset = yPos;
        int vertexCount = 0;
        
        int charSize = _fontTexture.Width / 16; // Size of one character in texture
        
        foreach (var section in formatString.Sections)
        {
            foreach (char c in section.Text)
            {
                if (c == '\n')
                {
                    xOffset = xPos;
                    yOffset += section.Size;
                    continue;
                }
                
                // Skip characters not in our font
                if (!_charPositions.ContainsKey(c))
                {
                    xOffset += section.Size;
                    continue;
                }
                
                // Get character position in texture
                Vector2 charPos = _charPositions[c];
                
                // Calculate texture coordinates (normalized 0-1)
                float u0 = charPos.X / _fontTexture.Width;
                float v0 = charPos.Y / _fontTexture.Height;
                float u1 = (charPos.X + charSize) / _fontTexture.Width;
                float v1 = (charPos.Y + charSize) / _fontTexture.Height;
                
                // Define quad vertices (4 vertices per character)
                // Vertex 0: Top-left
                _vertexData.AddRange([
                    xOffset, yOffset, 0.0f,  // position
                    u0, v0,                   // tex coord
                    section.ForegroundColor.X, section.ForegroundColor.Y, section.ForegroundColor.Z,  // foreground color
                    section.BackgroundColor.X, section.BackgroundColor.Y, section.BackgroundColor.Z   // background color
                ]);
                
                // Vertex 1: Top-right
                _vertexData.AddRange([
                    xOffset + section.Size, yOffset, 0.0f,
                    u1, v0,
                    section.ForegroundColor.X, section.ForegroundColor.Y, section.ForegroundColor.Z,
                    section.BackgroundColor.X, section.BackgroundColor.Y, section.BackgroundColor.Z
                ]);
                
                // Vertex 2: Bottom-right
                _vertexData.AddRange([
                    xOffset + section.Size, yOffset + section.Size, 0.0f,
                    u1, v1,
                    section.ForegroundColor.X, section.ForegroundColor.Y, section.ForegroundColor.Z,
                    section.BackgroundColor.X, section.BackgroundColor.Y, section.BackgroundColor.Z
                ]);
                
                // Vertex 3: Bottom-left
                _vertexData.AddRange([
                    xOffset, yOffset + section.Size, 0.0f,
                    u0, v1,
                    section.ForegroundColor.X, section.ForegroundColor.Y, section.ForegroundColor.Z,
                    section.BackgroundColor.X, section.BackgroundColor.Y, section.BackgroundColor.Z
                ]);
                
                // Add indices for two triangles forming a quad
                _indices.Add(vertexCount + 0);
                _indices.Add(vertexCount + 1);
                _indices.Add(vertexCount + 2);
                
                _indices.Add(vertexCount + 0);
                _indices.Add(vertexCount + 2);
                _indices.Add(vertexCount + 3);
                
                vertexCount += 4;
                xOffset += section.Size;
            }
        }
        
        // If there's no text to render, return early
        if (_vertexData.Count == 0)
        {
            return;
        }
        
        // Upload vertex data to GPU
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertexData.Count * sizeof(float), _vertexData.ToArray(), BufferUsage.DynamicDraw);
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(int), _indices.ToArray(), BufferUsage.DynamicDraw);
        
        // Create orthographic projection matrix for 2D text rendering
        // Maps from pixel coordinates to normalized device coordinates (-1 to 1)
        Matrix4d projection = Matrix4d.CreateOrthographicOffCenter(0, _screenWidth, _screenHeight, 0, -1, 1);
        
        // Set uniforms
        _shaderProgram.SetUniform("u_projection", projection);
        _shaderProgram.SetUniform("u_model", Matrix4d.Identity);
        
        // Draw the text
        GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
        
        GL.BindVertexArray(0);
    }
    
    public static void Render(string text, int size, Vector3 foregroundColor, Vector3 backgroundColor, Matrix4 transform, float xPos = 10, float yPos = 10)
    {
        // Build FormatString
        FormatString formatString = new();
        formatString.AddSection(new FormatStringSection(text, size, foregroundColor, backgroundColor, transform));
        Render(formatString, xPos, yPos);
    }
    
    public static void Render(string text, int size, Vector3 foregroundColor, Vector3 backgroundColor, float xPos = 10, float yPos = 10)
    {
        Render(text, size, foregroundColor, backgroundColor, Matrix4.Identity, xPos, yPos);
    }
    
    public static void Render(string text, int size, Vector3 foregroundColor, float xPos = 10, float yPos = 10)
    {
        Render(text, size, foregroundColor, Vector3.Zero, Matrix4.Identity, xPos, yPos);
    }
    
    public static void Render(string text, int size, float xPos = 10, float yPos = 10)
    {
        Render(text, size, Vector3.One, Vector3.Zero, Matrix4.Identity, xPos, yPos);
    }
    
    public static void Render(string text, float xPos = 10, float yPos = 10)
    {
        Render(text, 16, Vector3.One, Vector3.Zero, Matrix4.Identity, xPos, yPos);
    }
    
    
    
}