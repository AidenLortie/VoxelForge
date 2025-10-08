using VoxelForge.Shared.Networking;
using VoxelForge.Shared.Networking.Packets;
using Xunit;

namespace Shared.Test.Networking;

public class NetworkBridgeLocalTests
{
    [Fact]
    public void CheckPacket_Should_Transmit_Between_Connected_Bridges()
    {
        // Arrange
        var clientBridge = new NetworkBridgeLocal();
        var serverBridge = new NetworkBridgeLocal();
        clientBridge.ConnectTo(serverBridge);

        CheckPacket? received = null;
        serverBridge.RegisterHandler<CheckPacket>(p => received = p);

        var sent = new CheckPacket(timestamp: 123456789);

        // Act
        clientBridge.Send(sent);
        serverBridge.Poll();

        // Assert
        Assert.NotNull(received);
        Assert.Equal(sent.Timestamp, received!.Timestamp);
    }

    [Fact]
    public void Packet_Should_Not_Be_Received_Without_Poll()
    {
        var a = new NetworkBridgeLocal();
        var b = new NetworkBridgeLocal();
        a.ConnectTo(b);

        bool received = false;
        b.RegisterHandler<CheckPacket>(_ => received = true);

        a.Send(new CheckPacket(42));

        // No Poll yet — should still be queued
        Assert.False(received);

        // Now process
        b.Poll();
        Assert.True(received);
    }

    [Fact]
    public void Connection_Should_Be_Bidirectional()
    {
        var a = new NetworkBridgeLocal();
        var b = new NetworkBridgeLocal();
        a.ConnectTo(b);

        bool aGot = false, bGot = false;

        a.RegisterHandler<CheckPacket>(_ => aGot = true);
        b.RegisterHandler<CheckPacket>(_ => bGot = true);

        a.Send(new CheckPacket(1)); // A → B
        b.Send(new CheckPacket(2)); // B → A

        a.Poll();
        b.Poll();

        Assert.True(aGot);
        Assert.True(bGot);
    }
}