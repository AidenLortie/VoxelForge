using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace VoxelForge.Client.Rendering.Models;

/// <summary>
/// JSON-serializable model definition containing vertex and index data.
/// </summary>
public class ModelDefinition
{
    /// <summary>
    /// Name/ID of the model
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Vertex positions as flat array [x, y, z, x, y, z, ...]
    /// </summary>
    [JsonPropertyName("positions")]
    public float[] Positions { get; set; } = Array.Empty<float>();
    
    /// <summary>
    /// Vertex normals as flat array [x, y, z, x, y, z, ...]
    /// </summary>
    [JsonPropertyName("normals")]
    public float[] Normals { get; set; } = Array.Empty<float>();
    
    /// <summary>
    /// Texture coordinates as flat array [u, v, u, v, ...]
    /// </summary>
    [JsonPropertyName("texCoords")]
    public float[] TexCoords { get; set; } = Array.Empty<float>();
    
    /// <summary>
    /// Indices for indexed drawing
    /// </summary>
    [JsonPropertyName("indices")]
    public uint[] Indices { get; set; } = Array.Empty<uint>();
    
    /// <summary>
    /// Converts this model definition to an array of interleaved vertices.
    /// </summary>
    /// <returns>Array of Vertex structures with interleaved data.</returns>
    public Vertex[] ToVertices()
    {
        int vertexCount = Positions.Length / 3;
        var vertices = new Vertex[vertexCount];
        
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 position = new Vector3(
                Positions[i * 3],
                Positions[i * 3 + 1],
                Positions[i * 3 + 2]
            );
            
            Vector3 normal = Vector3.UnitY; // Default normal
            if (Normals.Length >= (i + 1) * 3)
            {
                normal = new Vector3(
                    Normals[i * 3],
                    Normals[i * 3 + 1],
                    Normals[i * 3 + 2]
                );
            }
            
            Vector2 texCoord = Vector2.Zero; // Default texcoord
            if (TexCoords.Length >= (i + 1) * 2)
            {
                texCoord = new Vector2(
                    TexCoords[i * 2],
                    TexCoords[i * 2 + 1]
                );
            }
            
            vertices[i] = new Vertex(position, normal, texCoord);
        }
        
        return vertices;
    }
}
