# VoxelForge Rendering System Roadmap

## Overview
This document outlines the complete development roadmap for integrating **OpenTK 5.0.0-pre.15** with the VoxelForge client system. The goal is to create a robust rendering system that can display the voxel world, handle chunk rendering efficiently, and eventually support entity and player rendering.

## Architecture Principles

### Current State
- **Server**: Authoritative world state, manages chunks and block updates
- **Client**: Maintains local world copy, handles networking, currently console-only
- **Shared**: Common code including World, Chunk, Block systems, networking, physics

### Target State
- **Server**: Continue managing authoritative world state, NO rendering responsibilities
- **Client**: Add OpenTK window, rendering pipeline, camera system, input handling
- **Shared**: Extend to support entity data transmission alongside chunk data

### Key Design Decisions
1. **Player Authority**: Initially player state lives on client, eventually migrate to server authority
2. **Rendering Location**: ALL rendering happens client-side only
3. **World Authority**: Server remains authoritative for world state (blocks)
4. **Entity Data Flow**: Extend chunk packet system to also transmit entity updates

---

## Phase 1: OpenTK Foundation & Window Management

### 1.1 Project Setup
**Files to Modify:**
- `Client/Client.csproj` - Add OpenTK 5.0.0-pre.15 NuGet package

**New Files:**
- `Client/Rendering/GameWindow.cs` - Main OpenTK window wrapper
- `Client/Rendering/IRenderer.cs` - Base renderer interface

**Tasks:**
- [ ] Add OpenTK 5.0.0-pre.15 package reference to Client project
- [ ] Create basic GameWindow class extending OpenTK.Windowing.Desktop.GameWindow
- [ ] Implement window initialization with proper OpenGL context (Core profile, version 3.3+)
- [ ] Set up basic window properties (size: 1280x720, title, vsync)
- [ ] Implement basic game loop (OnLoad, OnUpdateFrame, OnRenderFrame, OnUnload)
- [ ] Test window creation and basic OpenGL clear screen

**Dependencies:**
```xml
<PackageReference Include="OpenTK" Version="5.0.0-pre.15" />
```

### 1.2 Rendering Infrastructure
**New Files:**
- `Client/Rendering/RenderContext.cs` - Manages OpenGL state and context
- `Client/Rendering/Shader.cs` - Shader program wrapper
- `Client/Rendering/ShaderProgram.cs` - Manages shader compilation and linking

**Tasks:**
- [ ] Create RenderContext to manage OpenGL state (viewport, clear color, depth testing)
- [ ] Implement Shader class for loading and compiling vertex/fragment shaders
- [ ] Create basic shader utilities (uniform setting, attribute binding)
- [ ] Add error handling for shader compilation and linking
- [ ] Test with simple colored triangle rendering

### 1.3 Camera System
**New Files:**
- `Client/Rendering/Camera.cs` - Camera with view and projection matrices
- `Client/Input/InputManager.cs` - Handle keyboard and mouse input

**Tasks:**
- [ ] Implement Camera class with position, rotation (pitch, yaw)
- [ ] Calculate view matrix from camera transform
- [ ] Calculate perspective projection matrix
- [ ] Implement first-person camera controls (WASD movement, mouse look)
- [ ] Add camera movement speed and mouse sensitivity settings
- [ ] Test camera movement in empty world

**Camera Properties:**
- Field of view: 70 degrees
- Near plane: 0.1
- Far plane: 1000.0
- Movement speed: 5.0 units/second
- Mouse sensitivity: 0.002

---

## Phase 2: Chunk Rendering System

### 2.1 Mesh Generation
**New Files:**
- `Client/Rendering/ChunkMesh.cs` - Stores vertex data for a chunk
- `Client/Rendering/ChunkMeshBuilder.cs` - Generates mesh from chunk data
- `Client/Rendering/VertexFormat.cs` - Defines vertex structure

**Tasks:**
- [ ] Define vertex format (position: vec3, texCoord: vec2, normal: vec3, blockId: uint)
- [ ] Implement greedy meshing algorithm for efficient chunk rendering
- [ ] Only generate faces for blocks with air neighbors (face culling)
- [ ] Generate proper texture coordinates based on block type
- [ ] Calculate normals for lighting
- [ ] Optimize mesh generation to run off main thread
- [ ] Test mesh generation with single chunk

**Vertex Format:**
```glsl
// Position (3 floats) + TexCoord (2 floats) + Normal (3 floats) + BlockId (1 uint)
// Total: 9 floats per vertex, 36 bytes with padding
struct Vertex {
    vec3 position;
    vec2 texCoord;
    vec3 normal;
    uint blockId;
}
```

### 2.2 Chunk Renderer
**New Files:**
- `Client/Rendering/ChunkRenderer.cs` - Renders chunks using OpenGL
- `Client/Rendering/BufferManager.cs` - Manages VBO/VAO/EBO

**Tasks:**
- [ ] Create ChunkRenderer to manage rendering of all loaded chunks
- [ ] Implement VAO/VBO/EBO setup for chunk meshes
- [ ] Upload mesh data to GPU efficiently
- [ ] Render chunks with basic shader (position + color)
- [ ] Implement frustum culling to skip off-screen chunks
- [ ] Add chunk distance-based LOD system (future optimization)
- [ ] Test rendering multiple chunks from server

**OpenGL Objects Per Chunk:**
- 1 VAO (Vertex Array Object)
- 1 VBO (Vertex Buffer Object) for vertex data
- 1 EBO (Element Buffer Object) for indices (optional, for indexed drawing)

### 2.3 Texture System
**New Files:**
- `Client/Rendering/TextureAtlas.cs` - Manages block texture atlas
- `Client/Rendering/Texture.cs` - Single texture wrapper
- `Client/Content/Textures/` - Directory for texture assets

**Tasks:**
- [ ] Create texture atlas system (single texture with all block textures)
- [ ] Implement texture loading from files (PNG support via StbImageSharp)
- [ ] Generate texture coordinates for each block face type
- [ ] Add texture filtering (nearest for pixel art style, or linear)
- [ ] Implement texture binding in render loop
- [ ] Create placeholder textures for default blocks (air, stone, grass, dirt)
- [ ] Test textured chunk rendering

**Texture Atlas Layout:**
- 16x16 grid of 16x16 pixel textures (256x256 total)
- Supports 256 unique block textures
- Use texture coordinates (u, v) in range [0, 1]

### 2.4 Basic Lighting
**New Files:**
- `Client/Rendering/Shaders/chunk.vert` - Chunk vertex shader
- `Client/Rendering/Shaders/chunk.frag` - Chunk fragment shader

**Tasks:**
- [ ] Implement simple directional lighting in shaders
- [ ] Calculate lighting based on face normals (top brighter, sides medium, bottom darker)
- [ ] Add ambient occlusion approximation (darken vertices near corners)
- [ ] Support day/night cycle with varying light direction
- [ ] Test lighting on rendered chunks

**Lighting Model:**
```glsl
// Simple directional light + ambient
vec3 lightDir = normalize(vec3(0.5, 1.0, 0.3));
float diffuse = max(dot(normal, lightDir), 0.0);
vec3 ambient = vec3(0.3);
vec3 lighting = ambient + diffuse * vec3(1.0);
```

---

## Phase 3: Client-Side Player System

### 3.1 Player Entity (Client-Only)
**New Files:**
- `Client/Entity/ClientPlayer.cs` - Client-side player representation
- `Client/Entity/PlayerController.cs` - Handles player input and movement

**Tasks:**
- [ ] Create ClientPlayer class with position, rotation, velocity
- [ ] Implement player movement physics (walking, jumping, gravity)
- [ ] Add collision detection with world blocks (AABB collision)
- [ ] Integrate with camera (camera follows player position)
- [ ] Handle player actions (place block, break block)
- [ ] Send player position updates to server (future: for server authority)
- [ ] Test player movement and interaction

**Player Properties:**
- Bounding box: 0.6 x 1.8 x 0.6 (width x height x depth)
- Walk speed: 4.317 m/s
- Sprint speed: 5.612 m/s
- Jump velocity: 8.0 m/s
- Gravity: 32.0 m/s²
- Eye height: 1.62 (camera offset from player position)

### 3.2 Block Interaction
**Files to Modify:**
- `Client/Entity/PlayerController.cs` - Add raycasting for block selection
- `Client/Client.cs` - Integrate player actions with network packets

**Tasks:**
- [ ] Implement ray casting to determine which block player is looking at
- [ ] Visualize block selection (outline/highlight selected block)
- [ ] Handle left-click to break block (send UpdateBlockPacket with air)
- [ ] Handle right-click to place block (send UpdateBlockPacket with block type)
- [ ] Add block placement validation (can't place in occupied space)
- [ ] Update local world immediately for responsive feel (optimistic updates)
- [ ] Test block breaking and placing

**Raycast Parameters:**
- Max reach distance: 5.0 blocks
- Step size: 0.1 blocks (for accuracy)
- Check each step for block intersection

---

## Phase 4: Entity System Foundation

### 4.1 Entity Base Classes (Shared)
**New Files:**
- `Shared/Content/Entities/BaseEntity.cs` - Base class for all entities
- `Shared/Content/Entities/EntityType.cs` - Entity type enumeration
- `Shared/Content/Entities/EntityData.cs` - Serializable entity data

**Files to Modify:**
- `Shared/Content/Entity.cs` - Enhance existing Entity class

**Tasks:**
- [ ] Extend Entity base class with position, rotation, velocity
- [ ] Add entity type system (Player, Mob, Item, etc.)
- [ ] Implement entity data serialization for network transmission
- [ ] Create entity registry similar to BlockRegistry
- [ ] Add entity update flags (position changed, rotation changed, etc.)
- [ ] Test entity data serialization/deserialization

**Entity Data Structure:**
```csharp
public class EntityData {
    public Guid EntityId;
    public EntityType Type;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Velocity;
    public Dictionary<string, object> Properties;
}
```

### 4.2 Entity Network Packets
**New Files:**
- `Shared/Networking/Packets/EntitySpawnPacket.cs` - Spawn entity
- `Shared/Networking/Packets/EntityUpdatePacket.cs` - Update entity state
- `Shared/Networking/Packets/EntityDespawnPacket.cs` - Remove entity
- `Shared/Networking/Packets/EntityListPacket.cs` - List of entities in chunk

**Tasks:**
- [ ] Create EntitySpawnPacket for creating new entities
- [ ] Create EntityUpdatePacket for position/rotation/velocity updates
- [ ] Create EntityDespawnPacket for removing entities
- [ ] Create EntityListPacket to send all entities in a chunk
- [ ] Register packets in PacketRegistry
- [ ] Test packet serialization and transmission

**Packet Flow:**
```
Server → Client: EntityListPacket (on chunk load)
Server → Client: EntitySpawnPacket (new entity)
Server → Client: EntityUpdatePacket (entity moved)
Server → Client: EntityDespawnPacket (entity removed)
```

### 4.3 Client-Side Entity Manager
**New Files:**
- `Client/Entity/EntityManager.cs` - Manages all entities on client
- `Client/Entity/ClientEntity.cs` - Client-side entity representation

**Tasks:**
- [ ] Create EntityManager to store and update all entities
- [ ] Implement entity spawn/despawn handling
- [ ] Add entity interpolation for smooth movement between updates
- [ ] Handle entity updates from server
- [ ] Integrate with chunk system (entities belong to chunks)
- [ ] Test entity management with mock entities

**Entity Manager Responsibilities:**
- Store entities by ID (Dictionary<Guid, ClientEntity>)
- Update entity positions (interpolation)
- Handle entity lifecycle (spawn, update, despawn)
- Provide query methods (GetEntitiesInChunk, GetEntitiesInRadius)

---

## Phase 5: Entity Rendering

### 5.1 Entity Renderer
**New Files:**
- `Client/Rendering/EntityRenderer.cs` - Renders entities
- `Client/Rendering/EntityModel.cs` - Entity model representation
- `Client/Rendering/Shaders/entity.vert` - Entity vertex shader
- `Client/Rendering/Shaders/entity.frag` - Entity fragment shader

**Tasks:**
- [ ] Create EntityRenderer separate from ChunkRenderer
- [ ] Implement simple cube rendering for entities (placeholder)
- [ ] Support entity textures
- [ ] Add entity model loading (future: .obj or custom format)
- [ ] Implement entity billboarding (optional, for items)
- [ ] Add entity name tags (optional)
- [ ] Test entity rendering with spawned entities

**Simple Entity Model:**
```
Cube model:
- 8 vertices
- 6 faces (2 triangles each)
- Total: 36 vertices (non-indexed) or 8 vertices + 36 indices
```

### 5.2 Player Entity Rendering
**Files to Modify:**
- `Client/Rendering/EntityRenderer.cs` - Add player-specific rendering
- `Client/Entity/ClientPlayer.cs` - Add render data

**Tasks:**
- [ ] Create player model (simple humanoid for now)
- [ ] Implement player animation system (walking, jumping)
- [ ] Add player skin system (texture on player model)
- [ ] Render other players received from server (multiplayer prep)
- [ ] Add first-person model (player hands/arms visible)
- [ ] Implement third-person view toggle (optional)
- [ ] Test player rendering

**Player Model Structure:**
- Head: 8x8x8 cube
- Body: 8x12x4 cube
- Arms: 4x12x4 cubes (2)
- Legs: 4x12x4 cubes (2)
- Total: Simple humanoid figure

---

## Phase 6: Server Authority Migration

### 6.1 Player Authority on Server
**Files to Modify:**
- `Server/Server.cs` - Add player management
- `Client/Entity/ClientPlayer.cs` - Send input to server instead of direct control

**New Files:**
- `Server/Entity/ServerPlayer.cs` - Server-side player
- `Server/Entity/PlayerManager.cs` - Manages connected players

**Tasks:**
- [ ] Move player position authority to server
- [ ] Client sends input (movement direction, jump, actions) not position
- [ ] Server calculates player position, handles physics
- [ ] Server sends player updates to all clients
- [ ] Implement client-side prediction for responsive feel
- [ ] Add server-side validation (anti-cheat foundation)
- [ ] Test player movement with server authority

**Input Packet Structure:**
```csharp
public class PlayerInputPacket {
    public Vector3 MovementInput; // WASD as vector
    public bool Jump;
    public bool Sprint;
    public Vector2 LookDirection; // Mouse delta
}
```

### 6.2 Entity Authority on Server
**Files to Modify:**
- `Server/Server.cs` - Add entity spawning and management

**New Files:**
- `Server/Entity/EntityManager.cs` - Server-side entity manager
- `Server/Entity/ServerEntity.cs` - Server-side entity base

**Tasks:**
- [ ] Implement entity spawning on server
- [ ] Add entity AI/logic on server
- [ ] Broadcast entity updates to clients in range
- [ ] Implement entity persistence (save/load)
- [ ] Add entity physics on server
- [ ] Test entity spawning and updates

---

## Phase 7: Optimization & Polish

### 7.1 Rendering Optimizations
**Tasks:**
- [ ] Implement chunk mesh caching (rebuild only on change)
- [ ] Add multithreaded mesh generation
- [ ] Optimize frustum culling with spatial data structures (octree/BVH)
- [ ] Implement occlusion culling (don't render chunks behind others)
- [ ] Add chunk loading/unloading based on view distance
- [ ] Optimize OpenGL state changes (batch by shader/texture)
- [ ] Add render distance setting
- [ ] Profile and optimize hotspots

### 7.2 Networking Optimizations
**Tasks:**
- [ ] Implement chunk compression (future: as mentioned in IMPLEMENTATION_NOTES.md)
- [ ] Add delta updates for entities (send only changed properties)
- [ ] Implement entity update prioritization (closer = more frequent updates)
- [ ] Add network bandwidth monitoring
- [ ] Implement packet aggregation (multiple updates in one packet)
- [ ] Add client-side entity interpolation and extrapolation
- [ ] Test with simulated network latency

### 7.3 UI & HUD
**New Files:**
- `Client/UI/HUD.cs` - Heads-up display
- `Client/UI/Crosshair.cs` - Center screen crosshair
- `Client/UI/DebugOverlay.cs` - Debug information display

**Tasks:**
- [ ] Implement crosshair rendering
- [ ] Add debug overlay (FPS, position, chunk info, entity count)
- [ ] Create inventory UI (future)
- [ ] Add health/hunger bars (future)
- [ ] Implement chat system (future, multiplayer)
- [ ] Test UI rendering

---

## Phase 8: Advanced Features (Future)

### 8.1 Advanced Lighting
- [ ] Implement block light sources (torches, lava)
- [ ] Add sky light propagation
- [ ] Implement smooth lighting interpolation
- [ ] Add shadows (shadow mapping or simple ambient occlusion)
- [ ] Day/night cycle with sun/moon rendering

### 8.2 Advanced Entity Features
- [ ] Add entity animations (skeletal animation system)
- [ ] Implement entity AI (pathfinding, behaviors)
- [ ] Add entity-entity interactions
- [ ] Create mob spawning system
- [ ] Implement item entities (dropped items)

### 8.3 World Features
- [ ] Add particle system (block break effects, etc.)
- [ ] Implement weather system (rain, snow)
- [ ] Add biome tinting (grass/water color variation)
- [ ] Create world generation improvements
- [ ] Add structure generation

---

## Technical Considerations

### Performance Targets
- **FPS**: 60+ FPS with 8 chunk render distance
- **Chunk Generation**: < 16ms per chunk (non-blocking)
- **Network**: Support 100+ entity updates per second
- **Memory**: < 2GB RAM usage with 16 chunk render distance

### Compatibility
- **OpenGL Version**: 3.3 Core Profile minimum
- **Platform**: Windows, Linux, macOS (via OpenTK cross-platform support)
- **.NET Version**: .NET 9.0 (current project standard)

### Testing Strategy
- Unit tests for mesh generation algorithms
- Integration tests for networking packets
- Manual testing for rendering and gameplay
- Performance profiling for optimization
- Load testing for multiplayer scenarios

### Code Organization
```
Client/
├── Rendering/
│   ├── Camera.cs
│   ├── ChunkRenderer.cs
│   ├── EntityRenderer.cs
│   ├── GameWindow.cs
│   ├── RenderContext.cs
│   ├── Shader.cs
│   ├── Texture.cs
│   └── Shaders/
│       ├── chunk.vert
│       ├── chunk.frag
│       ├── entity.vert
│       └── entity.frag
├── Entity/
│   ├── ClientPlayer.cs
│   ├── ClientEntity.cs
│   ├── EntityManager.cs
│   └── PlayerController.cs
├── Input/
│   └── InputManager.cs
├── UI/
│   ├── HUD.cs
│   └── DebugOverlay.cs
└── Client.cs (integrate rendering system)

Server/
├── Entity/
│   ├── ServerPlayer.cs
│   ├── ServerEntity.cs
│   ├── EntityManager.cs
│   └── PlayerManager.cs
└── Server.cs

Shared/
├── Content/
│   └── Entities/
│       ├── BaseEntity.cs
│       ├── EntityType.cs
│       └── EntityData.cs
└── Networking/
    └── Packets/
        ├── EntitySpawnPacket.cs
        ├── EntityUpdatePacket.cs
        ├── EntityDespawnPacket.cs
        ├── EntityListPacket.cs
        └── PlayerInputPacket.cs
```

---

## Development Timeline Estimate

### Short-term (1-2 weeks)
- Phase 1: OpenTK Foundation & Window Management
- Phase 2: Chunk Rendering System
- Basic playable demo: Window with textured chunks

### Medium-term (3-4 weeks)
- Phase 3: Client-Side Player System
- Phase 4: Entity System Foundation
- Phase 5: Entity Rendering
- Playable game: Player can move and interact with world

### Long-term (5-8 weeks)
- Phase 6: Server Authority Migration
- Phase 7: Optimization & Polish
- Multiplayer-ready game with server authority

### Future (8+ weeks)
- Phase 8: Advanced Features
- Full-featured voxel game engine

---

## Dependencies & References

### NuGet Packages
- **OpenTK** 5.0.0-pre.15 - OpenGL bindings and windowing
- **StbImageSharp** (future) - Image loading for textures
- **System.Numerics** - Vector and Matrix math (already in .NET)

### External Resources
- OpenTK Documentation: https://opentk.net/
- OpenGL Reference: https://www.khronos.org/opengl/
- Greedy Meshing: https://0fps.net/2012/06/30/meshing-in-a-minecraft-game/
- Voxel Rendering: https://tomcc.github.io/2014/08/31/visibility-1.html

### Similar Projects for Reference
- Minecraft (voxel game inspiration)
- Minetest (open source voxel game)
- Terasology (open source voxel game engine)

---

## Open Questions & Decisions Needed

1. **Texture Style**: Pixel art (16x16) vs high-res? → Recommend 16x16 for classic voxel look
2. **Entity Models**: Custom format vs .obj vs voxel-based? → Start with simple cubes, add custom format later
3. **Physics Engine**: Use existing VoxelForge physics or integrate external library? → Use existing, extend as needed
4. **Networking Protocol**: Keep current TCP or add UDP for real-time updates? → Keep TCP for now, UDP is future optimization
5. **Chunk Loading Strategy**: Load all at once vs progressive? → Progressive loading based on view distance
6. **Lighting Model**: Simple vertex lighting vs per-pixel lighting? → Start simple, add complexity later

---

## Risk Assessment

### High Priority Risks
1. **Performance**: Mesh generation and rendering may be slow
   - Mitigation: Profile early, implement multithreading, use LOD
2. **Network Latency**: Entity updates may be laggy
   - Mitigation: Client-side prediction, interpolation
3. **Scope Creep**: Feature list is extensive
   - Mitigation: Stick to phased approach, complete phases before moving on

### Medium Priority Risks
1. **OpenTK Stability**: Pre-release version may have bugs
   - Mitigation: Test thoroughly, be ready to downgrade or report issues
2. **Complexity**: Rendering system is complex
   - Mitigation: Break into small tasks, test incrementally

### Low Priority Risks
1. **Cross-platform Issues**: Different behavior on different OS
   - Mitigation: Test on multiple platforms regularly

---

## Success Criteria

### Phase 1-2 Complete
- ✅ Window opens and displays textured chunks from server
- ✅ Camera can move freely through world
- ✅ 60+ FPS with multiple chunks

### Phase 3-5 Complete
- ✅ Player can walk, jump, and interact with blocks
- ✅ Entities visible and updated from server
- ✅ Block placement and breaking works

### Phase 6-7 Complete
- ✅ Multiplayer-ready with server authority
- ✅ Optimized and polished gameplay
- ✅ Basic UI/HUD functional

### Overall Success
- ✅ Complete voxel game client with rendering system
- ✅ Client-server architecture maintained
- ✅ Foundation for future advanced features
- ✅ Modular, maintainable codebase

---

## Conclusion

This roadmap provides a comprehensive path forward for integrating OpenTK 5.0.0-pre.15 with VoxelForge. The phased approach ensures steady progress while maintaining the existing client-server architecture. Each phase builds on the previous one, allowing for incremental testing and validation.

**Next Steps:**
1. Review this roadmap with team/maintainers
2. Begin Phase 1: Add OpenTK package and create basic window
3. Iterate through phases, testing thoroughly at each step
4. Update this document as implementation progresses and requirements evolve

---

*Last Updated: 2025-01-XX*
*Version: 1.0*
*Status: Planning Complete - Ready for Implementation*
