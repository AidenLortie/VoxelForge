using VoxelForge.Shared.Networking;
using VoxelForge.Shared.Networking.Packets;

namespace VoxelForge.Shared.Registry;

public class PacketRegistry
{
    public static readonly Dictionary<string, Func<Packet>> Factories = new()
    {
        ["ChunkData"] = () => new ChunkPacket()
    };
}