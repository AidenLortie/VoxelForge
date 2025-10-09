using OpenTK.Mathematics;

namespace VoxelForge.Client.Rendering.Models;

/// <summary>
/// Represents a vertex in a 3D model with position, normal, and texture coordinates.
/// Data is laid out for efficient interleaved vertex buffer usage.
/// </summary>
public struct Vertex
{
    // /// Position in 3D space (x, y, z)
    public Vector3 Position;
    
    // /// Normal vector for lighting (x, y, z)
    public Vector3 Normal;
    
    // /// Texture coordinates (u, v)
    public Vector2 TexCoord;
    
    // /// Creates a new vertex with the specified attributes.
    public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
    {
        Position = position;
        Normal = normal;
        TexCoord = texCoord;
    }
    
    // /// Size of vertex in bytes (8 floats: 3 position + 3 normal + 2 texcoord = 32 bytes)
    public const int SizeInBytes = sizeof(float) * 8;
    
    // /// Offset of position attribute in bytes
    public const int PositionOffset = 0;
    
    // /// Offset of normal attribute in bytes
    public const int NormalOffset = sizeof(float) * 3;
    
    // /// Offset of texcoord attribute in bytes
    public const int TexCoordOffset = sizeof(float) * 6;
}
