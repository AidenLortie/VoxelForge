using System.Collections.Concurrent;
using VoxelForge.Shared.Entities;

namespace VoxelForge.Shared.Entities;

/// <summary>
/// Manages all entities in the game world.
/// </summary>
public class EntityManager
{
    private readonly ConcurrentDictionary<int, Entity> _entities = new();
    private int _nextEntityId = 1;
    
    /// <summary>
    /// Spawns a new entity and assigns it a unique ID.
    /// </summary>
    /// <param name="entity">The entity to spawn</param>
    /// <returns>The assigned entity ID</returns>
    public int SpawnEntity(Entity entity)
    {
        int entityId = _nextEntityId++;
        entity.EntityId = entityId;
        _entities.TryAdd(entityId, entity);
        return entityId;
    }
    
    /// <summary>
    /// Spawns an entity with a specific ID (used for client-side entity sync).
    /// </summary>
    /// <param name="entity">The entity to spawn</param>
    /// <param name="entityId">The entity ID to use</param>
    public void SpawnEntity(Entity entity, int entityId)
    {
        entity.EntityId = entityId;
        _entities.TryAdd(entityId, entity);
        
        if (entityId >= _nextEntityId)
        {
            _nextEntityId = entityId + 1;
        }
    }
    
    /// <summary>
    /// Removes an entity from the world.
    /// </summary>
    /// <param name="entityId">ID of the entity to despawn</param>
    /// <returns>True if entity was removed, false if not found</returns>
    public bool DespawnEntity(int entityId)
    {
        return _entities.TryRemove(entityId, out _);
    }
    
    /// <summary>
    /// Gets an entity by ID.
    /// </summary>
    /// <param name="entityId">The entity ID</param>
    /// <returns>The entity, or null if not found</returns>
    public Entity? GetEntity(int entityId)
    {
        _entities.TryGetValue(entityId, out var entity);
        return entity;
    }
    
    /// <summary>
    /// Gets all entities in the world.
    /// </summary>
    /// <returns>Collection of all entities</returns>
    public IEnumerable<Entity> GetAllEntities()
    {
        return _entities.Values;
    }
    
    /// <summary>
    /// Updates all entities.
    /// </summary>
    /// <param name="deltaTime">Time since last update in seconds</param>
    public void UpdateAll(float deltaTime)
    {
        foreach (var entity in _entities.Values)
        {
            entity.Update(deltaTime);
        }
    }
    
    /// <summary>
    /// Gets the total number of entities.
    /// </summary>
    public int Count => _entities.Count;
    
    /// <summary>
    /// Clears all entities from the manager.
    /// </summary>
    public void Clear()
    {
        _entities.Clear();
        _nextEntityId = 1;
    }
}
