using Xunit;
using VoxelForge.Shared.Networking.Packets;
using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Test.Networking;

public class ChunkRequestPacketTests
{
    [Fact]
    public void ChunkRequestPacket_SerializeDeserialize_PreservesData()
    {
        // Arrange
        var packet = new ChunkRequestPacket(1.5f, 2.5f);

        // Act
        var compound = packet.Write();
        var deserializedPacket = new ChunkRequestPacket();
        deserializedPacket.Read(compound);

        // Assert
        Assert.Equal(1.5f, deserializedPacket.ChunkX);
        Assert.Equal(2.5f, deserializedPacket.ChunkZ);
    }
}
