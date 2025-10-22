using OpenTK.Graphics.OpenGL.Compatibility;
using StbImageSharp;

namespace VoxelForge.Client.Rendering;

public class Texture
{
    private int textureHandle;
    public int Width { get; private set; }
    public int Height { get; private set; }

    public Texture(string path)
    {
        // use STBImageSharp to load the texture, and set it up in OpenGL
        StbImage.stbi_set_flip_vertically_on_load(1);
        
        ImageResult image = ImageResult.FromStream(new FileStream(path, FileMode.Open));
        
        Width = image.Width;
        Height = image.Height;
        
        textureHandle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, textureHandle);
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.GenerateMipmap(TextureTarget.Texture2d);
        
        GL.BindTexture(TextureTarget.Texture2d, 0);
    }
    
    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2d, textureHandle);
    }
    
    public void Unbind()
    {
        GL.BindTexture(TextureTarget.Texture2d, 0);
    }
}