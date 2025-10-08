using System.Numerics;
using VoxelForge.Shared.Networking;
using VoxelForge.Shared.Networking.Packets;
using VoxelForge.Shared.World;
using VoxelForge.Shared.Content.Blocks;
using Xunit;

namespace Shared.Test.Networking;

public class ChunkPacketTests
{
    private Chunk CreateTestChunk()
    {
        var chunk = new Chunk(new Vector2(0, 0));

        // Fill subchunk[0] with alternating pattern
        var sub = chunk.SubChunks[0];
        for (int x = 0; x < 16; x++)
        for (int y = 0; y < 16; y++)
        for (int z = 0; z < 16; z++)
        {
            ushort id = (ushort)((x + y + z) % 2 == 0 ? 1 : 2);
            sub.SetBlockStateId(x, y, z, id);
        }

        return chunk;
    }

    [Fact]
    public void ChunkPacket_Should_Serialize_And_Deserialize_Correctly()
    {
        // Arrange
        var originalChunk = CreateTestChunk();
        var packet = new ChunkPacket(originalChunk);

        // Simulate serialization -> deserialization
        var tagCompound = packet.Write();

        var receivedPacket = new ChunkPacket();
        receivedPacket.Read(tagCompound);

        var reconstructed = receivedPacket.Chunk;

        // Act & Assert
        for (int i = 0; i < 16; i++)
        {
            var subA = originalChunk.SubChunks[i];
            var subB = reconstructed.SubChunks[i];

            for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
            for (int z = 0; z < 16; z++)
            {
                Assert.Equal(subA.GetBlockStateId(x, y, z), subB.GetBlockStateId(x, y, z));
            }
        }
    }

    [Fact]
    public void ChunkPacket_Should_Transmit_Through_LocalBridge()
    {
        // Arrange
        var serverBridge = new NetworkBridgeLocal();
        var clientBridge = new NetworkBridgeLocal();
        serverBridge.ConnectTo(clientBridge);

        var serverChunk = CreateTestChunk();

        Chunk? receivedChunk = null;
        clientBridge.RegisterHandler<ChunkPacket>(packet =>
        {
            packet.Read(packet.Write()); // normally this happens inside the network layer
            receivedChunk = packet.Chunk;
        });

        var packetToSend = new ChunkPacket(serverChunk);

        // Act
        serverBridge.Send(packetToSend);
        clientBridge.Poll();

        // Assert
        Assert.NotNull(receivedChunk);

        // Validate one subchunk pattern
        var origSub = serverChunk.SubChunks[0];
        var recvSub = receivedChunk!.SubChunks[0];

        for (int x = 0; x < 16; x++)
        for (int y = 0; y < 16; y++)
        for (int z = 0; z < 16; z++)
        {
            Assert.Equal(origSub.GetBlockStateId(x, y, z), recvSub.GetBlockStateId(x, y, z));
        }
    }

    [Fact]
    public void ChunkPacket_Should_Preserve_Chunk_Position()
    {
        // Arrange - Create chunk at a non-zero position
        var chunkPos = new Vector2(5, 3);
        var chunk = new Chunk(chunkPos);
        
        // Set blocks in multiple subchunks to verify they're placed correctly
        chunk.SubChunks[0].SetBlockStateId(1, 2, 3, 100);
        chunk.SubChunks[7].SetBlockStateId(4, 5, 6, 200);
        chunk.SubChunks[15].SetBlockStateId(7, 8, 9, 300);

        var packet = new ChunkPacket(chunk);

        // Act - Serialize and deserialize
        var tagCompound = packet.Write();
        var receivedPacket = new ChunkPacket();
        receivedPacket.Read(tagCompound);
        var receivedChunk = receivedPacket.Chunk;

        // Assert - Verify chunk position is preserved
        var originalWorldPos = chunk.GetWorldPosition();
        var receivedWorldPos = receivedChunk.GetWorldPosition();
        
        Assert.Equal(originalWorldPos.X, receivedWorldPos.X);
        Assert.Equal(originalWorldPos.Z, receivedWorldPos.Z);
        
        // Assert - Verify blocks are in correct subchunks (not all in SubChunks[0])
        Assert.Equal((ushort)100, receivedChunk.SubChunks[0].GetBlockStateId(1, 2, 3));
        Assert.Equal((ushort)200, receivedChunk.SubChunks[7].GetBlockStateId(4, 5, 6));
        Assert.Equal((ushort)300, receivedChunk.SubChunks[15].GetBlockStateId(7, 8, 9));
    }
}
