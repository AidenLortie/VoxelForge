using VoxelForge.Shared.Serialization.Tags;

namespace VoxelForge.Shared.Networking.Packets;

/// <summary>
/// Packet sent when a new entity spawns in the world.
/// </summary>
public class EntitySpawnPacket : Packet
{
    public override string Id => "EntitySpawn";
    
    public int EntityId { get; set; }
    public string EntityType { get; set; } = "";
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }
    public float RotX { get; set; }
    public float RotY { get; set; }
    public float RotZ { get; set; }
    
    public EntitySpawnPacket() { }
    
    public EntitySpawnPacket(int entityId, string entityType, float posX, float posY, float posZ, float rotX = 0, float rotY = 0, float rotZ = 0)
    {
        EntityId = entityId;
        EntityType = entityType;
        PosX = posX;
        PosY = posY;
        PosZ = posZ;
        RotX = rotX;
        RotY = rotY;
        RotZ = rotZ;
    }
    
    public override TagCompound Write()
    {
        var tag = new TagCompound(Id);
        tag.Add("EntityId", new TagInt(EntityId));
        tag.Add("EntityType", new TagString(EntityType));
        tag.Add("PosX", new TagFloat(PosX));
        tag.Add("PosY", new TagFloat(PosY));
        tag.Add("PosZ", new TagFloat(PosZ));
        tag.Add("RotX", new TagFloat(RotX));
        tag.Add("RotY", new TagFloat(RotY));
        tag.Add("RotZ", new TagFloat(RotZ));
        return tag;
    }
    
    public override void Read(TagCompound compound)
    {
        EntityId = ((TagInt)compound["EntityId"]).Value;
        EntityType = ((TagString)compound["EntityType"]).Value;
        PosX = ((TagFloat)compound["PosX"]).Value;
        PosY = ((TagFloat)compound["PosY"]).Value;
        PosZ = ((TagFloat)compound["PosZ"]).Value;
        RotX = ((TagFloat)compound["RotX"]).Value;
        RotY = ((TagFloat)compound["RotY"]).Value;
        RotZ = ((TagFloat)compound["RotZ"]).Value;
    }
}

/// <summary>
/// Packet sent when an entity updates (position, rotation, velocity).
/// Uses bit flags to only send changed properties.
/// </summary>
public class EntityUpdatePacket : Packet
{
    public override string Id => "EntityUpdate";
    
    [Flags]
    public enum UpdateFlags : byte
    {
        None = 0,
        Position = 1 << 0,
        Rotation = 1 << 1,
        Velocity = 1 << 2,
        OnGround = 1 << 3
    }
    
    public int EntityId { get; set; }
    public UpdateFlags Flags { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }
    public float RotX { get; set; }
    public float RotY { get; set; }
    public float RotZ { get; set; }
    public float VelX { get; set; }
    public float VelY { get; set; }
    public float VelZ { get; set; }
    public bool OnGround { get; set; }
    
    public EntityUpdatePacket() { }
    
    public override TagCompound Write()
    {
        var tag = new TagCompound(Id);
        tag.Add("EntityId", new TagInt(EntityId));
        tag.Add("Flags", new TagByte((byte)Flags));
        
        if (Flags.HasFlag(UpdateFlags.Position))
        {
            tag.Add("PosX", new TagFloat(PosX));
            tag.Add("PosY", new TagFloat(PosY));
            tag.Add("PosZ", new TagFloat(PosZ));
        }
        if (Flags.HasFlag(UpdateFlags.Rotation))
        {
            tag.Add("RotX", new TagFloat(RotX));
            tag.Add("RotY", new TagFloat(RotY));
            tag.Add("RotZ", new TagFloat(RotZ));
        }
        if (Flags.HasFlag(UpdateFlags.Velocity))
        {
            tag.Add("VelX", new TagFloat(VelX));
            tag.Add("VelY", new TagFloat(VelY));
            tag.Add("VelZ", new TagFloat(VelZ));
        }
        if (Flags.HasFlag(UpdateFlags.OnGround))
        {
            tag.Add("OnGround", new TagByte((byte)(OnGround ? 1 : 0)));
        }
        
        return tag;
    }
    
    public override void Read(TagCompound compound)
    {
        EntityId = ((TagInt)compound["EntityId"]).Value;
        Flags = (UpdateFlags)((TagByte)compound["Flags"]).Value;
        
        if (Flags.HasFlag(UpdateFlags.Position))
        {
            PosX = ((TagFloat)compound["PosX"]).Value;
            PosY = ((TagFloat)compound["PosY"]).Value;
            PosZ = ((TagFloat)compound["PosZ"]).Value;
        }
        if (Flags.HasFlag(UpdateFlags.Rotation))
        {
            RotX = ((TagFloat)compound["RotX"]).Value;
            RotY = ((TagFloat)compound["RotY"]).Value;
            RotZ = ((TagFloat)compound["RotZ"]).Value;
        }
        if (Flags.HasFlag(UpdateFlags.Velocity))
        {
            VelX = ((TagFloat)compound["VelX"]).Value;
            VelY = ((TagFloat)compound["VelY"]).Value;
            VelZ = ((TagFloat)compound["VelZ"]).Value;
        }
        if (Flags.HasFlag(UpdateFlags.OnGround))
        {
            OnGround = ((TagByte)compound["OnGround"]).Value == 1;
        }
    }
}

/// <summary>
/// Packet sent when an entity despawns/is removed from the world.
/// </summary>
public class EntityDespawnPacket : Packet
{
    public override string Id => "EntityDespawn";
    
    public int EntityId { get; set; }
    
    public EntityDespawnPacket() { }
    
    public EntityDespawnPacket(int entityId)
    {
        EntityId = entityId;
    }
    
    public override TagCompound Write()
    {
        var tag = new TagCompound(Id);
        tag.Add("EntityId", new TagInt(EntityId));
        return tag;
    }
    
    public override void Read(TagCompound compound)
    {
        EntityId = ((TagInt)compound["EntityId"]).Value;
    }
}
