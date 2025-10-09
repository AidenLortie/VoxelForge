# Model System Documentation

## Overview
The VoxelForge model system allows you to define 3D models using JSON files. Models are automatically loaded, cached, and reused for efficient rendering.

## JSON Model Format

### Structure
```json
{
  "name": "model_name",
  "positions": [x, y, z, x, y, z, ...],
  "normals": [x, y, z, x, y, z, ...],
  "texCoords": [u, v, u, v, ...],
  "indices": [i1, i2, i3, ...]
}
```

### Fields
- **name** (string): Unique identifier for the model
- **positions** (float[]): Vertex positions as flat array [x, y, z, x, y, z, ...]
- **normals** (float[]): Vertex normals as flat array [x, y, z, x, y, z, ...]
- **texCoords** (float[]): Texture coordinates as flat array [u, v, u, v, ...]
- **indices** (uint[]): Triangle indices for indexed drawing

### Data Layout
The model system uses **interleaved vertex data** for efficient GPU upload:
- Position (3 floats) + Normal (3 floats) + TexCoord (2 floats) = 8 floats per vertex
- Total: 32 bytes per vertex

### Vertex Attributes
When rendered, vertices are automatically set up with these attributes:
- **Location 0**: Position (vec3)
- **Location 1**: Normal (vec3)
- **Location 2**: TexCoord (vec2)

## Usage

### Loading from JSON File
```csharp
var modelManager = new ModelManager();
var model = modelManager.LoadFromFile("Models/cube.json");
```

### Loading from JSON String
```csharp
string json = File.ReadAllText("model.json");
var model = modelManager.LoadFromJson(json);
```

### Loading from Definition
```csharp
var definition = new ModelDefinition
{
    Name = "custom_model",
    Positions = new float[] { /* ... */ },
    Normals = new float[] { /* ... */ },
    TexCoords = new float[] { /* ... */ },
    Indices = new uint[] { /* ... */ }
};
var model = modelManager.LoadFromDefinition(definition);
```

### Rendering a Model
```csharp
// Bind shader and set uniforms
shader.Use();
shader.SetUniform("model", modelMatrix);
shader.SetUniform("view", viewMatrix);
shader.SetUniform("projection", projectionMatrix);

// Render the model
model.Render();
```

### Model Caching
Models are automatically cached by name. If you load the same model twice, the cached version is returned:
```csharp
var model1 = modelManager.LoadFromFile("Models/cube.json");
var model2 = modelManager.LoadFromFile("Models/cube.json");
// model1 and model2 point to the same cached model
```

### Getting a Cached Model
```csharp
var model = modelManager.GetModel("cube");
if (model != null)
{
    model.Render();
}
```

### Unloading Models
```csharp
// Unload a specific model
modelManager.UnloadModel("cube");

// Unload all models
modelManager.Dispose();
```

## Example: Cube Model

See `Models/cube.json` for a complete example of a simple cube model.

## Chunk Rendering

The chunk rendering system uses the same model infrastructure but generates geometry procedurally from voxel data:

1. **ChunkMeshBuilder** generates vertices and indices from chunk voxel data
2. **Model** class wraps the mesh data with VAO/VBO/EBO
3. **ChunkRenderer** manages all chunk models and renders them efficiently

### UV Coloring
As requested, the current shader uses UV coordinates to generate vertex colors:
```glsl
vec3 color = vec3(TexCoord.x, TexCoord.y, 0.5);
```
This creates a gradient effect based on the texture coordinates.

## Best Practices

1. **Use Indexed Drawing**: Always provide indices for efficient rendering
2. **Calculate Normals**: Proper normals are essential for lighting
3. **Normalize Normals**: Ensure normal vectors have unit length
4. **UV Range**: Keep texture coordinates in [0, 1] range
5. **Model Naming**: Use unique, descriptive names for each model
6. **Cache Models**: Load models once and reuse them throughout the application

## Advanced Features

### Custom Vertex Processing
The vertex shader receives:
- `aPosition` (location 0)
- `aNormal` (location 1)
- `aTexCoord` (location 2)

You can modify the shader to add custom vertex processing.

### Instanced Rendering (Future)
The model system is designed to support instanced rendering for drawing many copies of the same model efficiently.

## Technical Details

### GPU Memory
- Each model allocates:
  - 1 VAO (Vertex Array Object)
  - 1 VBO (Vertex Buffer Object)
  - 1 EBO (Element Buffer Object)
- Memory usage: ~32 bytes per vertex + 4 bytes per index

### Performance
- Models are uploaded to GPU once and reused
- Indexed drawing reduces vertex duplication
- Interleaved vertex data improves cache locality

## See Also
- `Client/Rendering/Models/Vertex.cs` - Vertex structure definition
- `Client/Rendering/Models/Model.cs` - Model class implementation
- `Client/Rendering/Models/ModelDefinition.cs` - JSON model definition
- `Client/Rendering/Models/ModelManager.cs` - Model caching and management
