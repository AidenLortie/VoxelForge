# VoxelForge

**VoxelForge** is a voxel-based game engine written in C# (.NET 9.0). It provides a modular, extensible architecture for building voxel games with client-server networking, physics simulation, and a flexible mod loading system.

## 🏗️ Architecture

VoxelForge is organized into several projects:

### Core Projects

- **Core** - Main entry point (currently minimal)
- **Client** - Client-side game application
- **Server** - Server-side game logic and world management
- **Shared** - Shared code used by both client and server

### Test Projects

- **Shared.Test** - Unit tests for the Shared library (50 tests)

## 🎮 Features

### World System
- **Voxel-based world** divided into chunks (16×256×16 blocks)
- **Chunks** are subdivided into 16 **SubChunks** (16×16×16 blocks each)
- Support for 65,536 unique block states per chunk
- Efficient block state management using a registry pattern

### Block System
- **Block types**: Air, Stone, Grass, Dirt (extensible)
- **Block states** with customizable properties
- **Block registry** for managing block types
- **BlockState registry** for managing block state IDs

### Networking
- **TCP-based client-server architecture** (default port: 25565)
- **Custom packet system** with serialization via Tag-based format
- **Bidirectional communication** between client and server
- Built-in packet types:
  - `CheckPacket` - Heartbeat/connectivity check
  - `ChunkPacket` - Chunk data transmission
  - `ChunkRequestPacket` - Request specific chunks
  - `UpdateBlockPacket` - Block state updates

### Physics
- **3D physics engine** with mesh-based collision detection
- **Vector2**, **Vector3**, and **Quaternion** math types
- **Dynamic and static physics bodies**
- Gravity simulation and force application
- Collision detection and response

### Mod System
- **Dynamic mod loading** from DLL files
- `IMod` interface for creating mods
- Mod lifecycle hooks:
  - `OnInitialize()` - Called when mod is loaded
  - `OnLoad()` - Called when game starts
  - `OnUnload()` - Called when game stops

### Event System
- **Event bus** for decoupled communication
- Support for custom game events

### Serialization
- **Tag-based serialization** (similar to NBT format)
- Support for multiple tag types: Byte, Short, Int, Long, Float, Double, String, List, Compound
- Efficient binary I/O

## 🚀 Getting Started

### Prerequisites

- **.NET 9.0 SDK** or later
- A C# IDE (Visual Studio, Rider, or VS Code)

### Building the Project

```bash
# Clone the repository
git clone https://github.com/AidenLortie/VoxelForge.git
cd VoxelForge

# Restore dependencies and build
dotnet restore
dotnet build

# Run tests
dotnet test
```

### Running the Server

```bash
cd Server
dotnet run
```

The server will start listening on `localhost:25565` and wait for client connections.

### Running the Client

```bash
cd Client
dotnet run
```

The client will connect to the server at `localhost:25565` and begin receiving chunk data.

## 📁 Project Structure

```
VoxelForge/
├── Client/              # Client application
│   ├── Client.cs        # Main client class
│   └── Client.csproj
├── Server/              # Server application
│   ├── Server.cs        # Main server class
│   └── Server.csproj
├── Core/                # Core entry point
│   ├── Program.cs
│   └── Core.csproj
├── Shared/              # Shared library
│   ├── Content/         # Game content (blocks, items, entities)
│   │   └── Blocks/      # Block system
│   ├── Events/          # Event system
│   ├── Lifecycle/       # Game lifecycle management
│   ├── Loader/          # Mod loading system
│   ├── Networking/      # Networking infrastructure
│   │   ├── NetworkBridge/
│   │   └── Packets/
│   ├── Physics/         # Physics engine
│   │   └── Math/        # Math utilities
│   ├── Registry/        # Registry system
│   ├── Serialization/   # Tag-based serialization
│   │   └── Tags/
│   ├── World/           # World and chunk management
│   └── Shared.csproj
└── Shared.Test/         # Unit tests
    └── Shared.Test.csproj
```

## 🔌 Creating a Mod

To create a mod for VoxelForge, implement the `IMod` interface:

```csharp
using VoxelForge.Shared;

public class MyMod : IMod
{
    public string ModId => "my_mod";
    public string Name => "My Awesome Mod";
    public string Version => "1.0.0";
    public string Author => "Your Name";
    public string Description => "A description of my mod";
    
    public void OnInitialize()
    {
        // Called when the mod is first loaded
        Console.WriteLine("My mod initialized!");
    }
    
    public void OnLoad()
    {
        // Called when the game is loading
        Console.WriteLine("My mod loaded!");
    }
    
    public void OnUnload()
    {
        // Called when the game is unloading
        Console.WriteLine("My mod unloaded!");
    }
}
```

Compile your mod as a DLL and load it using `ModLoader.Load("path/to/MyMod.dll")`.

## 🌐 Networking Protocol

### Connection Flow

1. **Server starts** and listens on port 25565
2. **Client connects** to the server
3. **Server sends initial world data** (all chunks) to the client
4. **Client stores chunks** in its local world representation
5. **Bidirectional packet exchange** begins

### Packet Types

#### CheckPacket
Heartbeat packet for testing connectivity. Contains a timestamp.

#### ChunkPacket
Transmits chunk data including:
- Chunk position (X, Z)
- All 16 subchunks with block state data
- Serialized using Tag format

#### ChunkRequestPacket
Client requests a specific chunk from the server:
```csharp
client.SendPacket(new ChunkRequestPacket(chunkX, chunkZ));
```

#### UpdateBlockPacket
Client or server sends block updates:
```csharp
client.SendPacket(new UpdateBlockPacket(x, y, z, blockStateId));
```

## 🔧 Development

### Running Tests

All 50 tests should pass:

```bash
dotnet test
```

### Code Style

The project follows standard C# naming conventions:
- PascalCase for public members
- camelCase for private fields (with `_` prefix)
- XML documentation comments on public APIs

### Adding New Block Types

1. Create a new class inheriting from `Block`:
```csharp
public class MyBlock : Block
{
    public MyBlock() : base("my_block") { }
}
```

2. Register in `DefaultBlocks.Initialize()`:
```csharp
var myBlock = new MyBlock();
BlockRegistry.Register(myBlock.Id, myBlock);
BlockStateRegistry.Register(myBlock.DefaultState());
```

## 📚 Key Classes

### World Management
- **`World`** - Manages collection of chunks
- **`Chunk`** - 16×256×16 block container with 16 subchunks
- **`SubChunk`** - 16×16×16 block container

### Block System
- **`Block`** - Base class for all block types
- **`BlockState`** - Represents a block with properties
- **`BlockRegistry`** - Central registry for block types
- **`BlockStateRegistry`** - Maps block states to numeric IDs

### Networking
- **`INetworkBridge`** - Abstraction for network communication
- **`NetworkBridgeNet`** - TCP-based implementation
- **`Packet`** - Base class for all network packets
- **`PacketRegistry`** - Factory registry for packet types

### Physics
- **`PhysicsEngine`** - Main physics simulation engine
- **`IPhysicsBody`** - Interface for physics-enabled objects
- **`DynamicPhysicsBody`** - Physics body affected by forces
- **`StaticPhysicsBody`** - Fixed physics body

### Math
- **`Vector2`** - 2D vector with common operations
- **`Vector3`** - 3D vector with common operations
- **`Quaternion`** - Rotation representation

## 📝 Implementation Notes

See [IMPLEMENTATION_NOTES.md](IMPLEMENTATION_NOTES.md) for detailed implementation history and technical details about the client-server world sync system.

## 🤝 Contributing

Contributions are welcome! Please ensure:
1. All tests pass (`dotnet test`)
2. Code builds without warnings
3. XML documentation is added to public APIs
4. New features include appropriate tests

## 📄 License

This project is in active development. License information will be added in a future update.

## 🎯 Roadmap

Future enhancements planned:
- [ ] Chunk compression for network efficiency
- [ ] Delta updates for partial chunk changes
- [ ] Client-side chunk caching and LOD
- [ ] Rendering system integration
- [ ] More block types with complex properties
- [ ] Server-side validation of block updates
- [ ] Multiplayer support with multiple clients
- [ ] World persistence and saving
- [ ] Biome generation system
- [ ] Entity system implementation

## 📧 Contact

For questions or issues, please use the GitHub issue tracker.

---

*VoxelForge - Building voxel worlds, one block at a time.*
