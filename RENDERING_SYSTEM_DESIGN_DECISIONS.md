# Rendering System Design Decisions

This document addresses the specific considerations mentioned in the issue and explains the design decisions made for the rendering system integration.

## Issue Requirements Addressed

### 1. How to Handle Rendering of Chunks

**Decision**: Client-side greedy meshing with progressive loading

**Rationale:**
- **Greedy Meshing**: Reduces triangle count by combining adjacent faces of the same block type into larger quads
- **Face Culling**: Only generate faces for blocks exposed to air, eliminating hidden geometry
- **Progressive Loading**: Load and render chunks based on view distance from player
- **Mesh Caching**: Cache generated meshes until chunk data changes

**Implementation Details:**
```csharp
// Chunk rendering pipeline
Client receives ChunkPacket from server
  ↓
ChunkMeshBuilder generates mesh (background thread)
  ↓
Mesh uploaded to GPU (VAO/VBO created)
  ↓
ChunkRenderer draws mesh each frame
  ↓
Frustum culling skips off-screen chunks
```

**Key Classes:**
- `ChunkRenderer`: Manages all chunk meshes and rendering
- `ChunkMeshBuilder`: Converts chunk data to vertex arrays
- `ChunkMesh`: Stores vertex data and OpenGL handles (VAO/VBO)

**Performance Considerations:**
- Mesh generation runs on background thread (non-blocking)
- Only rebuild mesh when chunk data changes
- Use indexed drawing (EBO) to reduce vertex duplication
- Implement distance-based LOD (future optimization)

**See**: RENDERING_SYSTEM_ROADMAP.md Phase 2 for detailed implementation steps

---

### 2. How to Pass Authority on Player to Server, Then Render on Client

**Decision**: Phased migration from client authority to server authority

**Current State (Phase 3)**:
- Player state (position, rotation, velocity) lives entirely on client
- Client calculates physics and movement locally
- Client has full authority over player
- Camera directly follows local player position

**Advantages of Client Authority**:
- Zero input latency (immediate response)
- No network lag for single-player experience
- Simpler initial implementation

**Target State (Phase 6)**:
- Server has authority over player position
- Client sends **input** (not position) to server
- Server calculates physics and validates movement
- Server broadcasts player updates to all clients
- Client uses prediction and interpolation for smooth experience

**Migration Strategy:**

**Step 1: Client Input Packet**
```csharp
// Client sends input instead of position
public class PlayerInputPacket {
    public Vector3 MovementInput;  // WASD as normalized vector
    public bool Jump;
    public bool Sprint;
    public Vector2 LookDirection;  // Mouse delta
    public uint SequenceNumber;    // For reconciliation
}
```

**Step 2: Server Calculates Position**
```csharp
// Server processes input
void OnPlayerInputPacket(PlayerInputPacket input) {
    var player = GetPlayer(clientId);
    
    // Apply input to player
    player.MovementInput = input.MovementInput;
    player.Jump = input.Jump;
    
    // Update physics (server-authoritative)
    player.UpdatePhysics(deltaTime);
    
    // Send result back to client
    Send(new PlayerUpdatePacket(player.Position, player.Velocity));
}
```

**Step 3: Client Prediction + Reconciliation**
```csharp
// Client predicts movement locally
void Update(deltaTime) {
    // Store input with sequence number
    var input = GatherInput();
    input.SequenceNumber = nextSequence++;
    pendingInputs.Add(input);
    
    // Apply input locally (prediction)
    ApplyInput(input, deltaTime);
    
    // Send to server
    SendPacket(input);
}

// Client reconciles with server
void OnPlayerUpdatePacket(PlayerUpdatePacket packet) {
    // Server says we're at X, but we predicted Y
    // Re-apply inputs after the one server processed
    
    player.Position = packet.Position;
    player.Velocity = packet.Velocity;
    
    // Remove acknowledged inputs
    pendingInputs.RemoveAll(i => i.SequenceNumber <= packet.SequenceNumber);
    
    // Replay inputs that weren't acknowledged yet
    foreach (var input in pendingInputs) {
        ApplyInput(input, deltaTime);
    }
}
```

**Rendering Implications:**
- Camera still follows player position (now server-authoritative)
- Client continues to render player with prediction
- Rendering code doesn't change, only data source changes
- Other players always render from server data (no prediction needed)

**Benefits of Server Authority:**
- Prevents cheating (speed hacks, teleportation)
- Consistent multiplayer experience
- Server can validate all movements
- Foundation for multiplayer gameplay

**See**: RENDERING_SYSTEM_ROADMAP.md Phase 6 for complete implementation

---

### 3. Extend Chunk and Chunk Data Passing to Also Pass Entity Data

**Decision**: Add entity packets alongside chunk packets, entities belong to chunks

**Current System:**
```
Server → Client: ChunkPacket (chunk blocks only)
Client stores chunk in local world
```

**Extended System:**
```
Server → Client: ChunkPacket (chunk blocks)
Server → Client: EntityListPacket (all entities in chunk)
Client stores chunk in local world
Client stores entities in EntityManager
```

**Entity-Chunk Relationship:**
- Entities logically belong to the chunk they're currently in
- When chunk loads, server sends all entities in that chunk
- When entity moves to different chunk, server sends update
- When chunk unloads, client despawns all entities in that chunk

**New Packet Types:**

**EntityListPacket** (sent with chunk):
```csharp
public class EntityListPacket : Packet {
    public int ChunkX;
    public int ChunkZ;
    public List<EntityData> Entities;
}

public class EntityData {
    public Guid EntityId;
    public EntityType Type;        // Player, Mob, Item, etc.
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Velocity;
    public Dictionary<string, object> Properties;
}
```

**EntitySpawnPacket** (new entity in loaded chunk):
```csharp
public class EntitySpawnPacket : Packet {
    public EntityData Entity;
}
```

**EntityUpdatePacket** (entity state changed):
```csharp
public class EntityUpdatePacket : Packet {
    public Guid EntityId;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Velocity;
    public byte UpdateFlags;  // Bit flags for what changed
}
```

**EntityDespawnPacket** (entity removed):
```csharp
public class EntityDespawnPacket : Packet {
    public Guid EntityId;
    public DespawnReason Reason;  // Died, Unloaded, Removed
}
```

**Modified Chunk Loading Flow:**
```csharp
// Server side
void OnClientConnected(clientId) {
    foreach (var chunk in world.GetLoadedChunks()) {
        // Send chunk data
        Send(clientId, new ChunkPacket(chunk));
        
        // Send entities in chunk
        var entities = entityManager.GetEntitiesInChunk(chunk.X, chunk.Z);
        Send(clientId, new EntityListPacket(chunk.X, chunk.Z, entities));
    }
}

// Client side
void OnChunkPacket(ChunkPacket packet) {
    // Store chunk as before
    var chunk = packet.Chunk;
    _world.SetChunk(chunk.X, chunk.Y, chunk.Z, chunk);
}

void OnEntityListPacket(EntityListPacket packet) {
    // Spawn all entities in this chunk
    foreach (var entityData in packet.Entities) {
        var entity = new ClientEntity(entityData);
        _entityManager.AddEntity(entity);
    }
}
```

**Incremental Updates:**
```csharp
// Server sends update when entity moves
void OnEntityMoved(entity) {
    var packet = new EntityUpdatePacket {
        EntityId = entity.Id,
        Position = entity.Position,
        UpdateFlags = UpdateFlags.Position
    };
    
    // Send to all clients with this chunk loaded
    var clients = GetClientsWithChunkLoaded(entity.ChunkX, entity.ChunkZ);
    foreach (var client in clients) {
        Send(client, packet);
    }
}
```

**Benefits:**
- Entities are automatically synchronized with chunks
- No entities render for unloaded chunks
- Efficient network usage (only send what's needed)
- Natural integration with existing chunk system

**See**: RENDERING_SYSTEM_ROADMAP.md Phase 4 for entity system implementation

---

### 4. Allow Updates Per Entity

**Decision**: Delta updates with bit flags for changed properties

**Problem**: Sending full entity state every update wastes bandwidth

**Solution**: Only send properties that changed

**EntityUpdatePacket with Flags:**
```csharp
[Flags]
public enum EntityUpdateFlags : byte {
    None = 0,
    Position = 1 << 0,      // 0x01
    Rotation = 1 << 1,      // 0x02
    Velocity = 1 << 2,      // 0x04
    Health = 1 << 3,        // 0x08
    Animation = 1 << 4,     // 0x10
    Properties = 1 << 5,    // 0x20
    All = 0xFF
}

public class EntityUpdatePacket : Packet {
    public Guid EntityId;
    public EntityUpdateFlags UpdateFlags;
    
    // Optional fields (only present if flag is set)
    public Vector3? Position;
    public Vector3? Rotation;
    public Vector3? Velocity;
    public float? Health;
    public string? AnimationState;
    public Dictionary<string, object>? Properties;
    
    public override TagCompound Write() {
        var compound = new TagCompound("EntityUpdate");
        compound.Add("EntityId", new TagString(EntityId.ToString()));
        compound.Add("Flags", new TagByte((byte)UpdateFlags));
        
        // Only serialize fields that are present
        if ((UpdateFlags & EntityUpdateFlags.Position) != 0)
            compound.Add("Position", SerializeVector3(Position.Value));
        if ((UpdateFlags & EntityUpdateFlags.Rotation) != 0)
            compound.Add("Rotation", SerializeVector3(Rotation.Value));
        if ((UpdateFlags & EntityUpdateFlags.Velocity) != 0)
            compound.Add("Velocity", SerializeVector3(Velocity.Value));
        // etc...
        
        return compound;
    }
}
```

**Update Frequency Optimization:**

Different update rates for different properties:
```csharp
// Server-side update manager
class EntityUpdateManager {
    // High frequency: Position (20 updates/sec)
    private Timer _positionTimer = new Timer(50);
    
    // Medium frequency: Velocity, Rotation (10 updates/sec)
    private Timer _movementTimer = new Timer(100);
    
    // Low frequency: Health, Properties (5 updates/sec)
    private Timer _stateTimer = new Timer(200);
    
    void Update() {
        if (_positionTimer.Elapsed()) {
            SendPositionUpdates();
        }
        if (_movementTimer.Elapsed()) {
            SendMovementUpdates();
        }
        if (_stateTimer.Elapsed()) {
            SendStateUpdates();
        }
    }
}
```

**Distance-Based Updates:**

Entities farther from player get less frequent updates:
```csharp
float GetUpdateFrequency(float distanceToPlayer) {
    if (distanceToPlayer < 10.0f)
        return 20.0f;  // 20 updates per second
    else if (distanceToPlayer < 30.0f)
        return 10.0f;  // 10 updates per second
    else if (distanceToPlayer < 60.0f)
        return 5.0f;   // 5 updates per second
    else
        return 1.0f;   // 1 update per second
}
```

**Client-Side Interpolation:**

Client smoothly interpolates between updates:
```csharp
class ClientEntity {
    private Vector3 _currentPosition;
    private Vector3 _targetPosition;
    private float _interpolationProgress = 0;
    private float _interpolationTime = 0.1f;  // 100ms
    
    void OnUpdateReceived(EntityUpdatePacket packet) {
        if ((packet.UpdateFlags & EntityUpdateFlags.Position) != 0) {
            _targetPosition = packet.Position.Value;
            _interpolationProgress = 0;
        }
    }
    
    void Update(float deltaTime) {
        if (_interpolationProgress < 1.0f) {
            _interpolationProgress += deltaTime / _interpolationTime;
            _currentPosition = Vector3.Lerp(_currentPosition, _targetPosition, 
                                           _interpolationProgress);
        }
    }
    
    void Render() {
        // Render at interpolated position
        RenderAt(_currentPosition);
    }
}
```

**Benefits:**
- Reduced bandwidth (only send changed properties)
- Scalable to many entities
- Smooth rendering with interpolation
- Distance-based optimization

**See**: RENDERING_SYSTEM_ROADMAP.md Phase 7.2 for network optimizations

---

### 5. Other Considerations

#### A. Chunk Render Distance Management

**Problem**: Can't render infinite chunks, need to manage what's loaded

**Solution**: View distance setting with progressive chunk loading

```csharp
class ChunkLoader {
    private int _viewDistance = 8;  // 8 chunks in each direction
    private Vector2 _lastPlayerChunk;
    
    void Update(Vector3 playerPosition) {
        var playerChunk = WorldToChunk(playerPosition);
        
        // Only update if player moved to different chunk
        if (playerChunk != _lastPlayerChunk) {
            UnloadDistantChunks(playerChunk);
            LoadNearbyChunks(playerChunk);
            _lastPlayerChunk = playerChunk;
        }
    }
    
    void LoadNearbyChunks(Vector2 centerChunk) {
        for (int x = -_viewDistance; x <= _viewDistance; x++) {
            for (int z = -_viewDistance; z <= _viewDistance; z++) {
                var chunkPos = centerChunk + new Vector2(x, z);
                
                // Skip if already loaded
                if (_loadedChunks.Contains(chunkPos))
                    continue;
                
                // Request from server
                _client.RequestChunk(chunkPos.X, chunkPos.Z);
            }
        }
    }
    
    void UnloadDistantChunks(Vector2 centerChunk) {
        var toUnload = new List<Vector2>();
        
        foreach (var chunkPos in _loadedChunks) {
            var distance = Vector2.Distance(chunkPos, centerChunk);
            if (distance > _viewDistance + 2) {  // Add buffer zone
                toUnload.Add(chunkPos);
            }
        }
        
        foreach (var chunkPos in toUnload) {
            UnloadChunk(chunkPos);
        }
    }
}
```

#### B. Multi-Threaded Mesh Generation

**Problem**: Mesh generation can be slow and block rendering

**Solution**: Generate meshes on worker threads

```csharp
class ChunkMeshGenerator {
    private ThreadPool _meshThreadPool;
    private ConcurrentQueue<ChunkMeshTask> _completedMeshes;
    
    void QueueMeshGeneration(Chunk chunk) {
        var task = new ChunkMeshTask { Chunk = chunk };
        _meshThreadPool.QueueWork(() => {
            task.Mesh = ChunkMeshBuilder.GenerateMesh(chunk);
            _completedMeshes.Enqueue(task);
        });
    }
    
    void Update() {
        // Process completed meshes on main thread (required for OpenGL)
        while (_completedMeshes.TryDequeue(out var task)) {
            UploadMeshToGPU(task.Mesh);
        }
    }
}
```

#### C. Player-Block Collision

**Problem**: Player needs to collide with world blocks

**Solution**: AABB collision detection

```csharp
class PlayerPhysics {
    private Vector3 _boundingBox = new Vector3(0.6f, 1.8f, 0.6f);
    
    bool CheckCollision(Vector3 position) {
        // Get min/max corners of player bounding box
        var min = position - _boundingBox / 2;
        var max = position + _boundingBox / 2;
        
        // Check all blocks player intersects
        for (int x = (int)Math.Floor(min.X); x <= (int)Math.Ceiling(max.X); x++) {
            for (int y = (int)Math.Floor(min.Y); y <= (int)Math.Ceiling(max.Y); y++) {
                for (int z = (int)Math.Floor(min.Z); z <= (int)Math.Ceiling(max.Z); z++) {
                    var block = _world.GetBlock(x, y, z);
                    if (block != null && block.Id != "air") {
                        return true;  // Collision!
                    }
                }
            }
        }
        return false;
    }
    
    Vector3 ResolveCollision(Vector3 position, Vector3 velocity) {
        // Try movement on each axis separately
        var newPos = position;
        
        // Try X
        newPos.X += velocity.X;
        if (CheckCollision(newPos))
            newPos.X = position.X;
        
        // Try Y
        newPos.Y += velocity.Y;
        if (CheckCollision(newPos))
            newPos.Y = position.Y;
        
        // Try Z
        newPos.Z += velocity.Z;
        if (CheckCollision(newPos))
            newPos.Z = position.Z;
        
        return newPos;
    }
}
```

#### D. Memory Management

**Problem**: Loading many chunks consumes significant memory

**Solution**: Unload distant chunks, use memory pools

```csharp
class MemoryManager {
    private ObjectPool<float[]> _vertexArrayPool;
    
    void UnloadChunk(Chunk chunk) {
        // Free GPU memory
        if (chunk.VAO != 0) {
            GL.DeleteVertexArray(chunk.VAO);
            GL.DeleteBuffer(chunk.VBO);
        }
        
        // Return arrays to pool for reuse
        if (chunk.VertexData != null) {
            _vertexArrayPool.Return(chunk.VertexData);
        }
        
        // Remove from loaded chunks
        _loadedChunks.Remove(chunk.Position);
    }
}
```

#### E. Cross-Platform Considerations

**OpenTK 5.0.0-pre.15 supports Windows, Linux, and macOS**

Platform-specific considerations:
- **Windows**: DirectX alternative exists but OpenGL is recommended
- **Linux**: Works well with Mesa drivers
- **macOS**: Deprecated OpenGL (3.3 max), future Metal backend needed

Testing strategy:
- Primary development on Windows
- Test on Linux regularly
- macOS support as best-effort (OpenGL 3.3 limit)

---

## Summary of Design Decisions

| Consideration | Decision | Rationale |
|---------------|----------|-----------|
| Chunk Rendering | Client-side greedy meshing | Reduces triangle count, efficient |
| Player Authority | Client → Server migration | Start simple, add multiplayer support |
| Entity Data | Extend chunk packets | Natural integration with chunk system |
| Entity Updates | Delta updates with flags | Efficient bandwidth usage |
| Render Distance | Progressive loading/unloading | Manage memory and performance |
| Threading | Multi-threaded mesh generation | Non-blocking mesh creation |
| Collision | AABB per-axis resolution | Simple and effective |
| Memory | Pooling and unloading | Prevent memory leaks |
| Platform | OpenGL 3.3 Core via OpenTK | Cross-platform support |

---

## Next Steps

1. Begin Phase 1: Add OpenTK package, create basic window
2. Implement chunk rendering (Phase 2)
3. Add player movement (Phase 3)
4. Extend with entity system (Phase 4-5)
5. Migrate to server authority (Phase 6)
6. Optimize and polish (Phase 7)

See [RENDERING_SYSTEM_ROADMAP.md](RENDERING_SYSTEM_ROADMAP.md) for complete implementation plan.

---

*This document explains the "why" behind the roadmap decisions. Keep it updated as implementation reveals new considerations.*
