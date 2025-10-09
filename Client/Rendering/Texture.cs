using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace VoxelForge.Client.Rendering;

/// <summary>
/// Represents a 2D texture loaded from an image file.
/// </summary>
public class Texture : IDisposable
{
    private int _handle;
    private bool _disposed = false;
    
    // /// Gets the OpenGL texture handle.
    public int Handle => _handle;
    
    // /// Gets the width of the texture in pixels.
    public int Width { get; private set; }
    
    // /// Gets the height of the texture in pixels.
    public int Height { get; private set; }
    
    // /// Loads a texture from a file.
    public Texture(string path)
    {
        // Generate texture
        _handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, _handle);
        
        // Load image using StbImage
        StbImage.stbi_set_flip_vertically_on_load(1);
        
        using (var stream = File.OpenRead(path))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            
            Width = image.Width;
            Height = image.Height;
            
            // Upload texture data
            GL.TexImage2D(
                TextureTarget.Texture2d,
                0,
                InternalFormat.Rgba,
                image.Width,
                image.Height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                image.Data
            );
        }
        
        // Set texture parameters
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        Console.WriteLine($"Texture loaded: {path} ({Width}x{Height})");
    }
    
    // /// Binds this texture to the specified texture unit.
    public void Bind(int unit = 0)
    {
        GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + unit));
        GL.BindTexture(TextureTarget.Texture2d, _handle);
    }
    
    // /// Unbinds any texture from the specified unit.
    public static void Unbind(int unit = 0)
    {
        GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + unit));
        GL.BindTexture(TextureTarget.Texture2d, 0);
    }
    
    // /// Disposes the texture and frees GPU resources.
    public void Dispose()
    {
        if (!_disposed)
        {
            GL.DeleteTexture(_handle);
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
    
    ~Texture()
    {
        if (!_disposed)
        {
            Console.WriteLine("Warning: Texture was not disposed properly");
        }
    }
}
