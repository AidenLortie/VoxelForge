using OpenTK.Mathematics;

namespace VoxelForge.Client.Rendering.Models;

/// <summary>
/// Represents a vertex in a 3D model with position, normal, and texture coordinates.
/// Data is laid out for efficient interleaved vertex buffer usage.
/// </summary>
public struct Vertex
{
    /// <summary>
    /// Position in 3D space (x, y, z)
    /// </summary>
    public Vector3 Position;
    
    /// <summary>
    /// Normal vector for lighting (x, y, z)
    /// </summary>
    public Vector3 Normal;
    
    /// <summary>
    /// Texture coordinates (u, v)
    /// </summary>
    public Vector2 TexCoord;
    
    /// <summary>
    /// Creates a new vertex with the specified attributes.
    /// </summary>
    public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
    {
        Position = position;
        Normal = normal;
        TexCoord = texCoord;
    }
    
    /// <summary>
    /// Size of vertex in bytes (8 floats: 3 position + 3 normal + 2 texcoord = 32 bytes)
    /// </summary>
    public const int SizeInBytes = sizeof(float) * 8;
    
    /// <summary>
    /// Offset of position attribute in bytes
    /// </summary>
    public const int PositionOffset = 0;
    
    /// <summary>
    /// Offset of normal attribute in bytes
    /// </summary>
    public const int NormalOffset = sizeof(float) * 3;
    
    /// <summary>
    /// Offset of texcoord attribute in bytes
    /// </summary>
    public const int TexCoordOffset = sizeof(float) * 6;
}
