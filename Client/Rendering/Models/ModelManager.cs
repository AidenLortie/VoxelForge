using System.Text.Json;

namespace VoxelForge.Client.Rendering.Models;

/// <summary>
/// Manages loading and caching of 3D models.
/// Provides model reuse to avoid duplicate GPU uploads.
/// </summary>
public class ModelManager : IDisposable
{
    private readonly Dictionary<string, Model> _models = new();
    private bool _disposed = false;
    
    // /// Loads a model from a JSON file.
    public Model LoadFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        var definition = JsonSerializer.Deserialize<ModelDefinition>(json);
        
        if (definition == null)
            throw new Exception($"Failed to deserialize model from {filePath}");
        
        return LoadFromDefinition(definition);
    }
    
    // /// Loads a model from a JSON string.
    public Model LoadFromJson(string json)
    {
        var definition = JsonSerializer.Deserialize<ModelDefinition>(json);
        
        if (definition == null)
            throw new Exception("Failed to deserialize model from JSON");
        
        return LoadFromDefinition(definition);
    }
    
    // /// Loads a model from a ModelDefinition.
    // If a model with the same name already exists, returns the cached model.
    public Model LoadFromDefinition(ModelDefinition definition)
    {
        // Check if model already loaded
        if (_models.TryGetValue(definition.Name, out var existingModel))
        {
            Console.WriteLine($"Model '{definition.Name}' already loaded, returning cached version");
            return existingModel;
        }
        
        // Create new model
        var model = Model.FromDefinition(definition);
        _models[definition.Name] = model;
        
        return model;
    }
    
    // /// Gets a cached model by name.
    public Model? GetModel(string name)
    {
        _models.TryGetValue(name, out var model);
        return model;
    }
    
    // /// Checks if a model with the given name is loaded.
    public bool HasModel(string name)
    {
        return _models.ContainsKey(name);
    }
    
    // /// Gets all loaded model names.
    public IEnumerable<string> GetModelNames()
    {
        return _models.Keys;
    }
    
    // /// Unloads a specific model.
    public void UnloadModel(string name)
    {
        if (_models.TryGetValue(name, out var model))
        {
            model.Dispose();
            _models.Remove(name);
            Console.WriteLine($"Model '{name}' unloaded");
        }
    }
    
    // /// Disposes all models and frees GPU resources.
    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (var model in _models.Values)
            {
                model.Dispose();
            }
            _models.Clear();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
