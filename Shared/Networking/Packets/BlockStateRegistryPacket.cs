using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Networking.Packets;

/// <summary>
/// Packet sent from server to client containing the BlockState registry mappings.
/// Ensures client and server have matching blockstate IDs.
/// </summary>
public class BlockStateRegistryPacket : Packet
{
    public override string Id => "BlockStateRegistry";
    
    /// <summary>
    /// Dictionary mapping blockstate IDs to their string representations (e.g., "stone", "grass[facing=north]")
    /// </summary>
    public Dictionary<ushort, string> StateIdToString { get; private set; } = new();

    public BlockStateRegistryPacket() { }

    public BlockStateRegistryPacket(Dictionary<ushort, string> stateIdToString)
    {
        StateIdToString = stateIdToString;
    }

    public override TagCompound Write()
    {
        var tag = new TagCompound(Id);
        tag.Add("Count", new TagInt(StateIdToString.Count));
        
        // Write each mapping as Id_N and State_N
        int index = 0;
        foreach (var kvp in StateIdToString)
        {
            tag.Add($"Id_{index}", new TagInt(kvp.Key));
            tag.Add($"State_{index}", new TagString(kvp.Value));
            index++;
        }
        
        return tag;
    }

    public override void Read(TagCompound compound)
    {
        StateIdToString.Clear();
        int count = ((TagInt)compound["Count"]).Value;
        
        for (int i = 0; i < count; i++)
        {
            ushort id = (ushort)((TagInt)compound[$"Id_{i}"]).Value;
            string state = ((TagString)compound[$"State_{i}"]).Value;
            StateIdToString[id] = state;
        }
    }
}
