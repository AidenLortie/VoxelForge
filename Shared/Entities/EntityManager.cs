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
    
    // /// Spawns a new entity and assigns it a unique ID.
    public int SpawnEntity(Entity entity)
    {
        int entityId = _nextEntityId++;
        entity.EntityId = entityId;
        _entities.TryAdd(entityId, entity);
        return entityId;
    }
    
    // /// Spawns an entity with a specific ID (used for client-side entity sync).
    public void SpawnEntity(Entity entity, int entityId)
    {
        entity.EntityId = entityId;
        _entities.TryAdd(entityId, entity);
        
        if (entityId >= _nextEntityId)
        {
            _nextEntityId = entityId + 1;
        }
    }
    
    // /// Removes an entity from the world.
    public bool DespawnEntity(int entityId)
    {
        return _entities.TryRemove(entityId, out _);
    }
    
    // /// Gets an entity by ID.
    public Entity? GetEntity(int entityId)
    {
        _entities.TryGetValue(entityId, out var entity);
        return entity;
    }
    
    // /// Gets all entities in the world.
    public IEnumerable<Entity> GetAllEntities()
    {
        return _entities.Values;
    }
    
    // /// Updates all entities.
    public void UpdateAll(float deltaTime)
    {
        foreach (var entity in _entities.Values)
        {
            entity.Update(deltaTime);
        }
    }
    
    // /// Gets the total number of entities.
    public int Count => _entities.Count;
    
    // /// Clears all entities from the manager.
    public void Clear()
    {
        _entities.Clear();
        _nextEntityId = 1;
    }
}
