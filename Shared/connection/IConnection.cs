namespace VoxelForge.Shared.connection;

public interface IConnection
{
    void SendPacket(byte[] data);
    byte[] ReceivePacket();
    void Close();
}