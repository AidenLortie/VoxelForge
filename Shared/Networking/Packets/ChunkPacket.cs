using System;
using System.Globalization;
using VoxelForge.Shared.Serialization.Tags;
using VoxelForge.Shared.World;

namespace VoxelForge.Shared.Networking.Packets
{
    public class ChunkPacket : Packet
    {
        public override string Id => "ChunkData";

        public Chunk Chunk { get; private set; }
        public TagCompound ChunkData { get; private set; }

        public ChunkPacket(Chunk chunk)
        {
            Chunk = chunk;
            ChunkData = new TagCompound("ChunkData");
            ChunkData.Add("PosX", new TagFloat(chunk.GetWorldPosition().X));
            ChunkData.Add("PosZ", new TagFloat(chunk.GetWorldPosition().Z));

            foreach (var subChunk in chunk.SubChunks)
            {
                var pos = subChunk.GetSubChunkRelativePosition();
                var subChunkKey = $"{pos.X},{pos.Y},{pos.Z}";

                var subChunkData = new TagCompound(subChunkKey);
                var blockStates = new TagByteArray("BlockStates");

                // Each block uses 2 bytes (ushort)
                byte[] blockStateIds = new byte[16 * 16 * 16 * 2];
                int index = 0;

                for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++)
                for (int z = 0; z < 16; z++)
                {
                    ushort stateId = subChunk.GetBlockStateId(x, y, z);
                    blockStateIds[index++] = (byte)(stateId & 0xFF);
                    blockStateIds[index++] = (byte)((stateId >> 8) & 0xFF);
                }

                blockStates.Value = blockStateIds;
                subChunkData.Add("BlockStates", blockStates);
                ChunkData.Add("" + subChunkKey, subChunkData);
            }
        }

        // Parameterless constructor for deserialization
        public ChunkPacket() { }

        public override TagCompound Write()
        {
            return ChunkData;
        }

        public override void Read(TagCompound compound)
        {
            ChunkData = compound;
            float posX = (compound["PosX"] as TagFloat)?.Value ?? 0;
            float posZ = (compound["PosZ"] as TagFloat)?.Value ?? 0;
            Chunk = new Chunk(new System.Numerics.Vector2(posX, posZ));

            foreach ((string key, var subChunkTag) in compound)
            {
                if (subChunkTag is not TagCompound subChunkData)
                    continue;

                // Parse subchunk key "x,y,z"
                var parts = subChunkData.Name.Split(',');
                if (parts.Length != 3) continue;

                int x = int.Parse(parts[0], CultureInfo.InvariantCulture);
                int y = int.Parse(parts[1], CultureInfo.InvariantCulture);
                int z = int.Parse(parts[2], CultureInfo.InvariantCulture);

                var subChunk = Chunk.GetOrCreateSubChunk(x * 16, y * 16, z * 16);
                var blockStates = subChunkData["BlockStates"] as TagByteArray;
                var bytes = blockStates?.Value;
                int index = 0;

                for (int bx = 0; bx < 16; bx++)
                for (int by = 0; by < 16; by++)
                for (int bz = 0; bz < 16; bz++)
                {
                    ushort stateId = (ushort)(bytes[index++] | (bytes[index++] << 8));
                    subChunk.SetBlockStateId(bx, by, bz, stateId);
                }
            }
        }
    }
}
