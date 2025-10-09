# Rendering System Quick Reference

This document provides a quick reference for developers implementing the rendering system. For complete details, see [RENDERING_SYSTEM_ROADMAP.md](RENDERING_SYSTEM_ROADMAP.md).

## Current Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         CLIENT                              │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  Rendering  │  │  Entity Mgr  │  │    Input     │      │
│  │   System    │  │   (Local)    │  │   Manager    │      │
│  └──────┬──────┘  └──────┬───────┘  └──────┬───────┘      │
│         │                 │                  │              │
│         └─────────────────┴──────────────────┘              │
│                           │                                 │
│                    ┌──────┴──────┐                         │
│                    │ Local World │                         │
│                    └──────┬──────┘                         │
│                           │                                 │
│                    ┌──────┴──────┐                         │
│                    │  Network    │                         │
│                    │   Bridge    │                         │
│                    └──────┬──────┘                         │
└───────────────────────────┼─────────────────────────────────┘
                            │ TCP/IP
                            │ Port 25565
┌───────────────────────────┼─────────────────────────────────┐
│                    ┌──────┴──────┐                         │
│                    │  Network    │                         │
│                    │   Bridge    │                         │
│                    └──────┬──────┘                         │
│                           │                                 │
│                    ┌──────┴──────┐                         │
│                    │Authoritative│                         │
│                    │    World    │                         │
│                    └─────────────┘                         │
│                                                             │
│                         SERVER                              │
└─────────────────────────────────────────────────────────────┘
```

## Quick Start Guide

### Phase 1: Get a Window Open
1. Add OpenTK NuGet package: `dotnet add package OpenTK --version 5.0.0-pre.15`
2. Create `Client/Rendering/GameWindow.cs`
3. Initialize OpenGL context (Core profile 3.3+)
4. Test with clear screen

### Phase 2: Render First Chunk
1. Create mesh from chunk data (greedy meshing)
2. Upload to GPU (VAO/VBO)
3. Create basic shader (vertex + fragment)
4. Render with view/projection matrices
5. Test with server-sent chunk

### Phase 3: Add Player
1. Create ClientPlayer class
2. Implement WASD + mouse movement
3. Add collision detection
4. Link camera to player position
5. Test player movement

## Key Classes & Responsibilities

### Client Side
- **GameWindow**: Main OpenTK window, game loop entry point
- **RenderContext**: Manages OpenGL state
- **ChunkRenderer**: Converts chunks to renderable meshes
- **EntityRenderer**: Renders entities (players, mobs, items)
- **Camera**: View and projection matrices, player viewport
- **ClientPlayer**: Local player state (position, rotation, velocity)
- **EntityManager**: Tracks all entities received from server
- **InputManager**: Keyboard and mouse input handling

### Server Side (No Rendering!)
- **Server**: Authoritative world state
- **ServerPlayer** (future): Player state managed by server
- **EntityManager** (future): Server-side entity management

### Shared
- **EntityData**: Serializable entity state for network transmission
- **EntitySpawnPacket**: Network packet for entity creation
- **EntityUpdatePacket**: Network packet for entity state updates
- **PlayerInputPacket**: Network packet for player input (future: server authority)

## Critical Design Patterns

### Mesh Generation (Greedy Meshing)
```csharp
// Only generate faces for blocks with air neighbors
for each block in chunk:
    if block is not air:
        if neighbor above is air:
            add top face to mesh
        if neighbor below is air:
            add bottom face to mesh
        // repeat for all 6 directions
```

### Render Loop
```csharp
OnRenderFrame():
    1. Clear screen (color + depth)
    2. Update camera matrices
    3. Bind shader program
    4. Set uniforms (view, projection)
    5. For each chunk in frustum:
        - Bind chunk VAO
        - Draw chunk mesh
    6. For each visible entity:
        - Bind entity VAO
        - Set model matrix
        - Draw entity mesh
    7. Render UI/HUD
    8. Swap buffers
```

### Entity Updates
```csharp
// Client receives entity update from server
OnEntityUpdatePacket(packet):
    entity = entityManager.GetEntity(packet.EntityId)
    if entity == null:
        // Entity doesn't exist, spawn it
        entity = entityManager.SpawnEntity(packet)
    
    // Update entity state
    entity.TargetPosition = packet.Position
    entity.TargetRotation = packet.Rotation
    
    // Interpolate smoothly in update loop
    Update(deltaTime):
        entity.Position = Lerp(entity.Position, entity.TargetPosition, deltaTime * 10)
        entity.Rotation = Slerp(entity.Rotation, entity.TargetRotation, deltaTime * 10)
```

## Performance Guidelines

### Do's ✅
- Use greedy meshing to reduce triangle count
- Implement frustum culling (don't render off-screen chunks)
- Batch render calls by shader and texture
- Generate meshes on background thread
- Cache chunk meshes (rebuild only on change)
- Use indexed drawing (EBO) to reduce vertex duplication

### Don'ts ❌
- Don't rebuild all chunk meshes every frame
- Don't render chunks behind the player
- Don't change OpenGL state unnecessarily
- Don't allocate memory in hot paths (cache buffers)
- Don't update GPU buffers more than necessary
- Don't render entities outside view distance

## Shader Examples

### Basic Chunk Vertex Shader
```glsl
#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;

out vec2 TexCoord;
out vec3 Normal;
out vec3 FragPos;

uniform mat4 view;
uniform mat4 projection;

void main() {
    FragPos = aPosition;
    TexCoord = aTexCoord;
    Normal = aNormal;
    gl_Position = projection * view * vec4(aPosition, 1.0);
}
```

### Basic Chunk Fragment Shader
```glsl
#version 330 core

in vec2 TexCoord;
in vec3 Normal;
in vec3 FragPos;

out vec4 FragColor;

uniform sampler2D blockTexture;
uniform vec3 lightDir = vec3(0.5, 1.0, 0.3);

void main() {
    // Sample texture
    vec4 texColor = texture(blockTexture, TexCoord);
    
    // Simple directional lighting
    float diffuse = max(dot(normalize(Normal), normalize(lightDir)), 0.0);
    vec3 ambient = vec3(0.3);
    vec3 lighting = ambient + diffuse * vec3(0.7);
    
    FragColor = vec4(texColor.rgb * lighting, texColor.a);
}
```

## Network Packet Flow

### Chunk Loading
```
Client connects
Server → Client: ChunkPacket (all loaded chunks)
Client: Generates meshes, uploads to GPU
Client: Renders chunks
```

### Entity Spawning
```
Server: Entity spawns in world
Server → Client: EntitySpawnPacket
Client: Creates ClientEntity
Client: Adds to EntityManager
Client: EntityRenderer picks up entity
```

### Entity Updates
```
Server: Entity position changes
Server → Client: EntityUpdatePacket (position, rotation, velocity)
Client: Updates target position
Client: Interpolates smoothly over time
```

### Block Interaction
```
Client: Player left-clicks block
Client: Raycasts to find target block
Client → Server: UpdateBlockPacket (block position, air)
Server: Updates world
Server → All Clients: UpdateBlockPacket
Clients: Update local world, regenerate chunk mesh
```

## Testing Checklist

### Phase 1 Tests
- [ ] Window opens without crashing
- [ ] OpenGL context initializes (check version)
- [ ] Clear screen to solid color works
- [ ] Window resizing works
- [ ] Game loop runs at stable FPS

### Phase 2 Tests
- [ ] Chunk mesh generates correctly
- [ ] Mesh uploads to GPU without errors
- [ ] Shader compiles and links
- [ ] Chunk renders with correct geometry
- [ ] Multiple chunks render correctly
- [ ] Frustum culling works (chunks outside view don't render)

### Phase 3 Tests
- [ ] Player can move with WASD
- [ ] Mouse look rotates camera
- [ ] Player collides with blocks
- [ ] Player can jump and falls with gravity
- [ ] Camera position follows player
- [ ] Player can't walk through walls

### Phase 4-5 Tests
- [ ] Entities spawn from server
- [ ] Entities render correctly
- [ ] Entity updates from server work
- [ ] Entity interpolation is smooth
- [ ] Multiple entities render correctly

### Phase 6 Tests
- [ ] Player input sends to server
- [ ] Server calculates player position
- [ ] Server sends position updates
- [ ] Client-side prediction works
- [ ] Other players render correctly

## Common Issues & Solutions

### Issue: Low FPS
**Causes:**
- Too many draw calls (not batching)
- Rendering off-screen chunks (no frustum culling)
- Regenerating meshes every frame
- Not using indexed drawing

**Solutions:**
- Implement frustum culling
- Cache chunk meshes
- Use instanced rendering for entities
- Profile and optimize hotspots

### Issue: Mesh Generation Too Slow
**Causes:**
- Not using greedy meshing
- Running on main thread
- Generating faces for hidden blocks

**Solutions:**
- Implement greedy meshing algorithm
- Move mesh generation to background thread
- Only generate faces with air neighbors

### Issue: Entities Appear Jittery
**Causes:**
- No interpolation between updates
- Update rate too low
- Network latency

**Solutions:**
- Implement position interpolation
- Use client-side prediction
- Extrapolate position based on velocity

### Issue: Texture Looks Wrong
**Causes:**
- Incorrect texture coordinates
- Texture not bound
- Wrong texture filtering

**Solutions:**
- Verify texture coordinate calculation
- Ensure texture is bound before draw call
- Set texture filtering (GL_NEAREST for pixel art)

## Useful OpenTK Commands

```csharp
// Shader creation
int shader = GL.CreateShader(ShaderType.VertexShader);
GL.ShaderSource(shader, sourceCode);
GL.CompileShader(shader);

// Program linking
int program = GL.CreateProgram();
GL.AttachShader(program, vertexShader);
GL.AttachShader(program, fragmentShader);
GL.LinkProgram(program);

// Buffer creation
int vbo = GL.GenBuffer();
GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), 
              vertices, BufferUsageHint.StaticDraw);

// VAO setup
int vao = GL.GenVertexArray();
GL.BindVertexArray(vao);
GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 
                       8 * sizeof(float), 0);
GL.EnableVertexAttribArray(0);

// Drawing
GL.UseProgram(program);
GL.BindVertexArray(vao);
GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
```

## Resource Links

- **Full Roadmap**: [RENDERING_SYSTEM_ROADMAP.md](RENDERING_SYSTEM_ROADMAP.md)
- **Implementation Notes**: [IMPLEMENTATION_NOTES.md](IMPLEMENTATION_NOTES.md)
- **OpenTK Docs**: https://opentk.net/
- **OpenGL Reference**: https://docs.gl/
- **Greedy Meshing Article**: https://0fps.net/2012/06/30/meshing-in-a-minecraft-game/

## Next Steps

1. Read the full roadmap: [RENDERING_SYSTEM_ROADMAP.md](RENDERING_SYSTEM_ROADMAP.md)
2. Start with Phase 1: Add OpenTK and create basic window
3. Progress through phases incrementally
4. Test thoroughly at each step
5. Update documentation as you implement

---

*This is a living document. Update as implementation progresses.*
