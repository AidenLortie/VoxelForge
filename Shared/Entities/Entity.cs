using System.Numerics;

namespace VoxelForge.Shared.Entities;

/// <summary>
/// Base class for all entities in the game world (players, mobs, items, etc.)
/// </summary>
public abstract class Entity
{
    // /// Unique identifier for this entity.
    public int EntityId { get; set; }
    
    // /// Position of the entity in world space.
    public Vector3 Position { get; set; }
    
    // /// Rotation of the entity (pitch, yaw, roll) in radians.
    public Vector3 Rotation { get; set; }
    
    // /// Velocity of the entity in blocks per second.
    public Vector3 Velocity { get; set; }
    
    // /// Type identifier for this entity (e.g., "player", "zombie", "item_drop")
    public abstract string EntityType { get; }
    
    // /// Whether this entity is on the ground.
    public bool OnGround { get; set; }
    
    // /// Age of the entity in ticks (for animations, despawn timers, etc.)
    public int Age { get; set; }
    
    protected Entity()
    {
        Position = Vector3.Zero;
        Rotation = Vector3.Zero;
        Velocity = Vector3.Zero;
        OnGround = false;
        Age = 0;
    }
    
    // /// Updates the entity. Called once per game tick.
    public virtual void Update(float deltaTime)
    {
        Age++;
        
        // Apply velocity to position
        Position += Velocity * deltaTime;
    }
    
    // /// Serializes entity-specific data for network transmission.
    // Override this to add custom entity data.
    public virtual Dictionary<string, object> SerializeData()
    {
        return new Dictionary<string, object>();
    }
    
    // /// Deserializes entity-specific data from network transmission.
    // Override this to read custom entity data.
    public virtual void DeserializeData(Dictionary<string, object> data)
    {
        // Override in subclasses
    }
}
