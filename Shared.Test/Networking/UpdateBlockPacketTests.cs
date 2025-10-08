using Xunit;
using VoxelForge.Shared.Networking.Packets;
using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Test.Networking;

public class UpdateBlockPacketTests
{
    [Fact]
    public void UpdateBlockPacket_SerializeDeserialize_PreservesData()
    {
        // Arrange
        var packet = new UpdateBlockPacket(10, 20, 30, 42);

        // Act
        var compound = packet.Write();
        var deserializedPacket = new UpdateBlockPacket();
        deserializedPacket.Read(compound);

        // Assert
        Assert.Equal(10, deserializedPacket.X);
        Assert.Equal(20, deserializedPacket.Y);
        Assert.Equal(30, deserializedPacket.Z);
        Assert.Equal((ushort)42, deserializedPacket.BlockStateId);
    }
}
