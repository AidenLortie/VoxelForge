# Client-Server World Sync Implementation

## Overview
This implementation provides a complete client-server world synchronization system for VoxelForge.

## Features Implemented

### 1. Default Block Definitions
Created four basic block types:
- **AirBlock** (id: "air")
- **StoneBlock** (id: "stone")
- **GrassBlock** (id: "grass")
- **DirtBlock** (id: "dirt")

All blocks are registered in `BlockRegistry` and their default states are registered in `BlockStateRegistry` via `DefaultBlocks.Initialize()`.

### 2. New Packet Types

#### ChunkRequestPacket
Allows clients to request specific chunks from the server.
```csharp
public ChunkRequestPacket(float chunkX, float chunkZ)
```

#### UpdateBlockPacket
Allows clients to send block updates to the server.
```csharp
public UpdateBlockPacket(int x, int y, int z, ushort blockStateId)
```

### 3. Server Implementation

The server now:
1. **Initializes default blocks** on startup
2. **Maintains a World object** containing chunks
3. **Sends all chunks on client connection** (initial world sync)
4. **Handles ChunkRequestPacket** to send specific chunks on demand
5. **Handles UpdateBlockPacket** to update blocks in the world

#### Example Server Flow:
```
1. Server starts and listens on port 25565
2. Client connects
3. Server sends all chunks immediately (initial sync)
4. Server polls for incoming packets
5. On ChunkRequestPacket: Server sends requested chunk
6. On UpdateBlockPacket: Server updates block in world
```

### 4. Client Implementation

The client now:
1. **Initializes default blocks** on startup
2. **Maintains a local World object** to store received chunks
3. **Handles ChunkPacket** to store chunks locally
4. **Can request specific chunks** via `RequestChunk(chunkX, chunkZ)`
5. **Can send block updates** via `UpdateBlock(x, y, z, blockStateId)`

#### Example Client Flow:
```
1. Client connects to server on port 25565
2. Client receives initial chunks from server
3. Client stores chunks in local world
4. Client can request additional chunks as needed
5. Client can send block updates to server
```

### 5. World Class Enhancements

Added new methods to the `World` class:
- `SetChunk(x, y, z, chunk)` - Store a chunk in the world
- `GetAllChunks()` - Retrieve all non-null chunks

## Communication Protocol

### On Client Connect:
```
Server → Client: ChunkPacket (for each chunk in world)
```

### Client Requesting a Chunk:
```
Client → Server: ChunkRequestPacket(chunkX, chunkZ)
Server → Client: ChunkPacket (if chunk exists)
```

### Client Updating a Block:
```
Client → Server: UpdateBlockPacket(x, y, z, blockStateId)
Server: Updates block in world
```

## Testing

All tests pass (50 total):
- Existing tests continue to work
- New packet serialization tests added
- Manual testing confirmed bidirectional communication

### Manual Test Results:
```
✓ Server successfully listens on port 25565
✓ Client successfully connects
✓ Server sends initial chunk on connection
✓ Client receives and stores chunk
✓ Client can request specific chunks
✓ Server responds to chunk requests
✓ Check packets work bidirectionally
```

## Usage Example

### Server:
```csharp
var server = new Server(networkBridge);
await server.RunAsync();
// Server automatically sends chunks on client connection
// Server handles ChunkRequestPacket and UpdateBlockPacket
```

### Client:
```csharp
var client = new Client(networkBridge);
client.StartListening();
// Client receives initial chunks automatically
client.RequestChunk(0, 0); // Request specific chunk
client.UpdateBlock(5, 10, 7, 2); // Update block at (5,10,7) to state 2
```

## Next Steps

Future enhancements could include:
- Chunk compression for network efficiency
- Delta updates for partial chunk changes
- Client-side chunk caching
- More sophisticated block types with properties
- Validation of block updates on server
