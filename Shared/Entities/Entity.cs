using System.Numerics;

namespace VoxelForge.Shared.Entities;

/// <summary>
/// Base class for all entities in the game world (players, mobs, items, etc.)
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Unique identifier for this entity.
    /// </summary>
    public int EntityId { get; set; }
    
    /// <summary>
    /// Position of the entity in world space.
    /// </summary>
    public Vector3 Position { get; set; }
    
    /// <summary>
    /// Rotation of the entity (pitch, yaw, roll) in radians.
    /// </summary>
    public Vector3 Rotation { get; set; }
    
    /// <summary>
    /// Velocity of the entity in blocks per second.
    /// </summary>
    public Vector3 Velocity { get; set; }
    
    /// <summary>
    /// Type identifier for this entity (e.g., "player", "zombie", "item_drop")
    /// </summary>
    public abstract string EntityType { get; }
    
    /// <summary>
    /// Whether this entity is on the ground.
    /// </summary>
    public bool OnGround { get; set; }
    
    /// <summary>
    /// Age of the entity in ticks (for animations, despawn timers, etc.)
    /// </summary>
    public int Age { get; set; }
    
    protected Entity()
    {
        Position = Vector3.Zero;
        Rotation = Vector3.Zero;
        Velocity = Vector3.Zero;
        OnGround = false;
        Age = 0;
    }
    
    /// <summary>
    /// Updates the entity. Called once per game tick.
    /// </summary>
    /// <param name="deltaTime">Time since last update in seconds</param>
    public virtual void Update(float deltaTime)
    {
        Age++;
        
        // Apply velocity to position
        Position += Velocity * deltaTime;
    }
    
    /// <summary>
    /// Serializes entity-specific data for network transmission.
    /// Override this to add custom entity data.
    /// </summary>
    /// <returns>Dictionary of key-value pairs representing entity data</returns>
    public virtual Dictionary<string, object> SerializeData()
    {
        return new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Deserializes entity-specific data from network transmission.
    /// Override this to read custom entity data.
    /// </summary>
    /// <param name="data">Dictionary of key-value pairs representing entity data</param>
    public virtual void DeserializeData(Dictionary<string, object> data)
    {
        // Override in subclasses
    }
}
