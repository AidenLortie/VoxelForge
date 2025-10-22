using OpenTK.Graphics.OpenGL;

namespace VoxelForge.Client.Rendering;

public static class TextureRepository
{
    public static Dictionary<string, Texture> Textures { get; } = new();
    public static int MaxGlTextureCount { get; set; }

    public static void Init()
    {
        // Query the maximum number of texture units supported by the GPU
        GL.GetInteger(GetPName.MaxTextureImageUnits, out int maxTextureUnits);
        MaxGlTextureCount = maxTextureUnits;
    }
    
    public static Texture GetTexture(string name)
    {
        return Textures.TryGetValue(name, out var texture) ? texture : throw new ArgumentException("Texture not found.");
    }
    
    public static bool HasTexture(string name)
    {
        return Textures.ContainsKey(name);
    }
    
    public static void AddTexture(string name, Texture texture)
    {
        if (Textures.Count >= MaxGlTextureCount)
        {
            throw new InvalidOperationException("Maximum texture count reached. Cannot add more textures.");
        }
        
        Textures.TryAdd(name, texture);
    }
    
    public static void RemoveTexture(string name)
    {
        Textures.Remove(name);
    }
    
    public static void Clear()
    {
        Textures.Clear();
    }
}