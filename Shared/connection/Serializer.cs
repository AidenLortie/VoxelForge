using System.Numerics;
using VoxelForge.Shared.World;

namespace VoxelForge.Shared.connection;

public enum PayloadType : byte
{
    SubChunk = 0,
    Chunk = 1,
    BlockUpdate = 2,
    Entity = 3
}

public static class Serializer
{
    public static void Serialize(SubChunk subChunk, BinaryWriter writer)
    {
        for (var x = 0; x < SubChunk.Size; x++)
        for (var y = 0; y < SubChunk.Size; y++)
        for (var z = 0; z < SubChunk.Size; z++)
        {
            ushort block = subChunk.GetBlockStateId(x, y, z);
            writer.Write(block);
        }
    }

    public static SubChunk Deserialize(BinaryReader reader)
    {
        ushort[,,] blocks = new ushort[SubChunk.Size, SubChunk.Size, SubChunk.Size];
        for (var x = 0; x < SubChunk.Size; x++)
        for (var y = 0; y < SubChunk.Size; y++)
        for (var z = 0; z < SubChunk.Size; z++)
        {
            blocks[x, y, z] = reader.ReadUInt16();
        }
        return new SubChunk(new Vector3(0, 0, 0), blocks); // Position will need to be set appropriately after deserialization
    }
    
    public static void Serialize(Chunk chunk, BinaryWriter writer)
    {
        for (var i = 0; i < 16; i++)
        {
            var subChunk = chunk.SubChunks[i];
            Serialize(subChunk, writer);
        }
    }
    
    public static Chunk DeserializeChunk(BinaryReader reader, Vector2 chunkPosition, World.World world)
    {
        var chunk = new Chunk(chunkPosition, world);
        for (var i = 0; i < 16; i++)
        {
            var subChunk = Deserialize(reader);
            chunk.SubChunks[i] = subChunk;
        }
        return chunk;
    }
    
    
}