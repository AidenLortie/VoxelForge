using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using VoxelForge.Client.Rendering.Models;
using VoxelForge.Shared.Entities;

namespace VoxelForge.Client.Rendering;

// Renders entities using JSON models with interpolation for smooth movement
public class EntityRenderer
{
    private readonly Dictionary<int, EntityRenderData> _renderData = new();
    private readonly ModelManager _modelManager;
    private ShaderProgram? _entityShader;
    
    public EntityRenderer(ModelManager modelManager)
    {
        _modelManager = modelManager;
    }
    
    // Initialize entity rendering (load shaders, models)
    public void Initialize()
    {
        // Load entity shader
        string vertSource = File.ReadAllText("Assets/Shaders/entity.vert");
        string fragSource = File.ReadAllText("Assets/Shaders/entity.frag");
        _entityShader = new ShaderProgram(vertSource, fragSource);
        
        Console.WriteLine("EntityRenderer initialized");
    }
    
    // Add entity to renderer
    public void AddEntity(Entity entity)
    {
        if (_renderData.ContainsKey(entity.EntityId))
            return;
            
        _renderData[entity.EntityId] = new EntityRenderData
        {
            Entity = entity,
            InterpolatedPosition = new OpenTK.Mathematics.Vector3(entity.Position.X, entity.Position.Y, entity.Position.Z),
            InterpolatedRotation = new OpenTK.Mathematics.Vector3(entity.Rotation.X, entity.Rotation.Y, entity.Rotation.Z),
        };
        
        Console.WriteLine($"Added entity {entity.EntityId} ({entity.EntityType}) to renderer");
    }
    
    // Remove entity from renderer
    public void RemoveEntity(int entityId)
    {
        if (_renderData.Remove(entityId))
        {
            Console.WriteLine($"Removed entity {entityId} from renderer");
        }
    }
    
    // Update entity data (called when entity update received from server)
    public void UpdateEntity(Entity entity)
    {
        if (_renderData.TryGetValue(entity.EntityId, out var data))
        {
            data.Entity = entity;
            // Don't reset interpolated values - let them smoothly catch up
        }
    }
    
    // Update interpolation (called every frame)
    public void Update(float deltaTime)
    {
        const float interpolationSpeed = 10.0f; // How fast to catch up to server position
        
        foreach (var data in _renderData.Values)
        {
            // Smoothly interpolate position
            var targetPos = new OpenTK.Mathematics.Vector3(
                data.Entity.Position.X, 
                data.Entity.Position.Y, 
                data.Entity.Position.Z);
            var positionDelta = targetPos - data.InterpolatedPosition;
            data.InterpolatedPosition += positionDelta * interpolationSpeed * deltaTime;
            
            // Smoothly interpolate rotation
            var targetRot = new OpenTK.Mathematics.Vector3(
                data.Entity.Rotation.X, 
                data.Entity.Rotation.Y, 
                data.Entity.Rotation.Z);
            var rotationDelta = targetRot - data.InterpolatedRotation;
            data.InterpolatedRotation += rotationDelta * interpolationSpeed * deltaTime;
        }
    }
    
    // Render all entities
    public void Render(Matrix4 view, Matrix4 projection)
    {
        if (_entityShader == null)
            return;
            
        _entityShader.Use();
        _entityShader.SetUniform("view", view);
        _entityShader.SetUniform("projection", projection);
        
        foreach (var data in _renderData.Values)
        {
            RenderEntity(data);
        }
    }
    
    // Render single entity
    private void RenderEntity(EntityRenderData data)
    {
        // Get model for entity type
        Model? model = data.Entity.EntityType switch
        {
            "player" => _modelManager.GetModel("player") ?? LoadPlayerModel(),
            _ => _modelManager.GetModel("cube"), // Default cube
        };
        
        if (model == null || _entityShader == null)
            return;
        
        // Build model matrix (translate, rotate, scale)
        var modelMatrix = Matrix4.Identity;
        modelMatrix *= Matrix4.CreateScale(0.6f, 1.8f, 0.6f); // Player dimensions
        modelMatrix *= Matrix4.CreateRotationY(data.InterpolatedRotation.Y);
        modelMatrix *= Matrix4.CreateTranslation(data.InterpolatedPosition);
        
        _entityShader.SetUniform("model", modelMatrix);
        _entityShader.SetUniform("entityColor", new OpenTK.Mathematics.Vector3(1.0f, 0.8f, 0.6f)); // Skin-ish color
        
        model.Render();
    }
    
    // Load player model on demand
    private Model? LoadPlayerModel()
    {
        try
        {
            return _modelManager.LoadFromFile("Assets/Models/player.json");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load player model: {ex.Message}");
            return null;
        }
    }
    
    // Clean up resources
    public void Dispose()
    {
        _entityShader?.Dispose();
    }
    
    // Data for rendering an entity with interpolation
    private class EntityRenderData
    {
        public Entity Entity { get; set; } = null!;
        public OpenTK.Mathematics.Vector3 InterpolatedPosition { get; set; }
        public OpenTK.Mathematics.Vector3 InterpolatedRotation { get; set; }
    }
}
