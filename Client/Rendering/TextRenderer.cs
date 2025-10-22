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
        GL.GenVertexArray(out _vao);
        GL.GenBuffer(out _vbo);
        GL.GenBuffer(out _ebo);
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        

    }
    
    
    
    public static void Render(FormatString formatString)
    {
        _shaderProgram.Use();
        _fontTexture.Bind();
        _vertexData.Clear();
        _indices.Clear();
        
        int xOrigin = 0;
        int yOrigin = 0;
        int xOffset = 0;
        int yOffset = 0;
        
        
        foreach (var section in formatString.Sections)
        {
            foreach (char c in section.Text)
            {
                for(int i = 0; i < 4; i++)
                {
                    // Calculate vertex positions and texture coordinates
                    float x = xOrigin + xOffset + ((i == 0 || i == 3) ? 0 : section.Size);
                    float y = yOrigin + yOffset + ((i < 2) ? 0 : section.Size);
                    float z = 0;
                    float u = _charPositions[c].X / _fontTexture.Width + ((i == 0 || i == 3) ? 0 : section.Size) / (float)_fontTexture.Width;
                    float v = _charPositions[c].Y / _fontTexture.Height + ((i < 2) ? 0 : section.Size) / (float)_fontTexture.Height;  
                    float fr = section.ForegroundColor.X;
                    float fg = section.ForegroundColor.Y;
                    float fb = section.ForegroundColor.Z;
                    float br = section.BackgroundColor.X;
                    float bg = section.BackgroundColor.Y;
                    float bb = section.BackgroundColor.Z;
                    
                    _vertexData.AddRange([x, y, z, u, v, fr, fg, fb, br, bg, bb]);
                    _indices.Add(xOrigin + xOffset + ((i == 0 || i == 3) ? 0 : section.Size));
                    _indices.Add(xOrigin + xOffset + ((i == 1 || i == 2) ? 0 : section.Size));
                    _indices.Add(xOrigin + xOffset + ((i == 0 || i == 3) ? 0 : section.Size));
                    _indices.Add(xOrigin + xOffset + ((i == 1 || i == 2) ? 0 : section.Size));
                    _indices.Add(xOrigin + xOffset + ((i == 2 || i == 3) ? 0 : section.Size));
                    _indices.Add(xOrigin + xOffset + ((i == 1 || i == 2) ? 0 : section.Size));
                    xOffset += section.Size;
                    
                }
            }
        }
    }
    
    public static void Render(string text, int size, Vector3 foregroundColor, Vector3 backgroundColor, Matrix4 transform)
    {
        // Build FormatString
        FormatString formatString = new();
        formatString.AddSection(new FormatStringSection(text, size, foregroundColor, backgroundColor, transform));
        Render(formatString);
    }
    
    public static void Render(string text, int size, Vector3 foregroundColor, Vector3 backgroundColor)
    {
        Render(text, size, foregroundColor, backgroundColor, Matrix4.Identity);
    }
    
    public static void Render(string text, int size, Vector3 foregroundColor)
    {
        Render(text, size, foregroundColor, Vector3.Zero, Matrix4.Identity);
    }
    
    public static void Render(string text, int size)
    {
        Render(text, size, Vector3.One, Vector3.Zero, Matrix4.Identity);
    }
    
    public static void Render(string text)
    {
        Render(text, 12, Vector3.One, Vector3.Zero, Matrix4.Identity);
    }
    
    
    
}