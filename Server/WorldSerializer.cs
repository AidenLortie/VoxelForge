using VoxelForge.Shared.Serialization.Tags;
using VoxelForge.Shared.World;

namespace VoxelForge.Server;

/// <summary>
/// Handles serialization and deserialization of world data to/from disk.
/// </summary>
public class WorldSerializer
{
    private readonly string _savesDirectory;

    public WorldSerializer(string savesDirectory = "Saves")
    {
        _savesDirectory = savesDirectory;
        Directory.CreateDirectory(_savesDirectory);
    }

    // /// Saves a world to disk with the specified seed.
    public void Save(World world, int seed)
    {
        string filePath = Path.Combine(_savesDirectory, $"world_{seed}.dat");
        Console.WriteLine($"Saving world to {filePath}...");

        var worldTag = new TagCompound("World");
        worldTag.Add("Seed", new TagInt(seed));
        worldTag.Add("SizeX", new TagInt(world.SizeX));
        worldTag.Add("SizeY", new TagInt(world.SizeY));
        worldTag.Add("SizeZ", new TagInt(world.SizeZ));

        // Save all chunks
        var chunksTag = new TagCompound("Chunks");
        foreach (var chunk in world.GetAllChunks())
        {
            var chunkPos = chunk.GetChunkPosition();
            var chunkKey = $"{(int)chunkPos.X},{(int)chunkPos.Y}";
            
            var chunkTag = new TagCompound(chunkKey);
            chunkTag.Add("PosX", new TagFloat(chunk.GetWorldPosition().X));
            chunkTag.Add("PosZ", new TagFloat(chunk.GetWorldPosition().Z));

            // Save all subchunks
            foreach (var subChunk in chunk.SubChunks)
            {
                var subPos = subChunk.GetSubChunkRelativePosition();
                var subChunkKey = $"{subPos.X},{subPos.Y},{subPos.Z}";

                var subChunkTag = new TagCompound(subChunkKey);
                
                // Save block states as byte array (2 bytes per block = ushort)
                byte[] blockStates = new byte[16 * 16 * 16 * 2];
                int index = 0;
                for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++)
                for (int z = 0; z < 16; z++)
                {
                    ushort stateId = subChunk.GetBlockStateId(x, y, z);
                    blockStates[index++] = (byte)(stateId & 0xFF);
                    blockStates[index++] = (byte)((stateId >> 8) & 0xFF);
                }

                subChunkTag.Add("BlockStates", new TagByteArray("BlockStates") { Value = blockStates });
                chunkTag.Add(subChunkKey, subChunkTag);
            }

            chunksTag.Add(chunkKey, chunkTag);
        }
        worldTag.Add("Chunks", chunksTag);

        // Write to file
        using var fileStream = File.Create(filePath);
        using var writer = new BinaryWriter(fileStream);
        worldTag.Write(writer);

        Console.WriteLine($"World saved successfully!");
    }

    // /// Loads a world from disk if it exists for the specified seed.
    public World? Load(int seed)
    {
        string filePath = Path.Combine(_savesDirectory, $"world_{seed}.dat");
        
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"No save file found for seed {seed}");
            return null;
        }

        Console.WriteLine($"Loading world from {filePath}...");

        try
        {
            using var fileStream = File.OpenRead(filePath);
            using var reader = new BinaryReader(fileStream);
            
            var worldTag = new TagCompound("World");
            worldTag.Read(reader);

            int loadedSeed = ((TagInt)worldTag["Seed"]).Value;
            if (loadedSeed != seed)
            {
                Console.WriteLine($"Warning: Save file seed ({loadedSeed}) doesn't match requested seed ({seed})");
            }

            int sizeX = ((TagInt)worldTag["SizeX"]).Value;
            int sizeY = ((TagInt)worldTag["SizeY"]).Value;
            int sizeZ = ((TagInt)worldTag["SizeZ"]).Value;

            var world = new World(sizeX, sizeY, sizeZ);

            // Load all chunks
            var chunksTag = (TagCompound)worldTag["Chunks"];
            foreach (var chunkEntry in chunksTag)
            {
                var chunkTag = (TagCompound)chunkEntry.Value;
                float posX = ((TagFloat)chunkTag["PosX"]).Value;
                float posZ = ((TagFloat)chunkTag["PosZ"]).Value;

                var chunk = new Chunk(new System.Numerics.Vector2(posX, posZ));

                // Load all subchunks
                foreach (var subChunkEntry in chunkTag)
                {
                    if (subChunkEntry.Key == "PosX" || subChunkEntry.Key == "PosZ")
                        continue;

                    var subChunkTag = (TagCompound)subChunkEntry.Value;
                    var blockStatesTag = (TagByteArray)subChunkTag["BlockStates"];
                    byte[] blockStates = blockStatesTag.Value;

                    // Parse subchunk position from key
                    var parts = subChunkEntry.Key.Split(',');
                    int subX = int.Parse(parts[0]);
                    int subY = int.Parse(parts[1]);
                    int subZ = int.Parse(parts[2]);

                    // Find the matching subchunk
                    foreach (var subChunk in chunk.SubChunks)
                    {
                        var subPos = subChunk.GetSubChunkRelativePosition();
                        if (subPos.X == subX && subPos.Y == subY && subPos.Z == subZ)
                        {
                            // Restore block states
                            int index = 0;
                            for (int x = 0; x < 16; x++)
                            for (int y = 0; y < 16; y++)
                            for (int z = 0; z < 16; z++)
                            {
                                ushort stateId = (ushort)(blockStates[index++] | (blockStates[index++] << 8));
                                subChunk.SetBlockStateId(x, y, z, stateId);
                            }
                            break;
                        }
                    }
                }

                // Add chunk to world
                var chunkPos = chunk.GetChunkPosition();
                world.SetChunk((int)chunkPos.X, 0, (int)chunkPos.Y, chunk);
            }

            Console.WriteLine($"World loaded successfully!");
            return world;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading world: {ex.Message}");
            return null;
        }
    }

    // /// Checks if a world save exists for the specified seed.
    public bool SaveExists(int seed)
    {
        string filePath = Path.Combine(_savesDirectory, $"world_{seed}.dat");
        return File.Exists(filePath);
    }
}
