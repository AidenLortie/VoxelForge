using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace VoxelForge.Client.Rendering.Models;

/// <summary>
/// JSON-serializable model definition containing vertex and index data.
/// </summary>
public class ModelDefinition
{
    // /// Name/ID of the model
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    
    // /// Vertex positions as flat array [x, y, z, x, y, z, ...]
    [JsonPropertyName("positions")]
    public float[] Positions { get; set; } = Array.Empty<float>();
    
    // /// Vertex normals as flat array [x, y, z, x, y, z, ...]
    [JsonPropertyName("normals")]
    public float[] Normals { get; set; } = Array.Empty<float>();
    
    // /// Texture coordinates as flat array [u, v, u, v, ...]
    [JsonPropertyName("texCoords")]
    public float[] TexCoords { get; set; } = Array.Empty<float>();
    
    // /// Indices for indexed drawing
    [JsonPropertyName("indices")]
    public uint[] Indices { get; set; } = Array.Empty<uint>();
    
    // /// Converts this model definition to an array of interleaved vertices.
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
