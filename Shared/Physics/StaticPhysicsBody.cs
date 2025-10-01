using VoxelForge.Shared.Physics.Math;

namespace VoxelForge.Shared.Physics;

public class StaticPhysicsBody : IPhysicsBody
{
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 BoundingBoxSize { get; set; }
    
    
    public void ApplyForce(Vector3 force)
    {
        return; // Static bodies do not respond to forces
    }

    public void ApplyImpulse(Vector3 impulse)
    {
        return; // Static bodies do not respond to impulses
    }

    public void Update(float deltaTime, PhysicsEngine engine)
    {
        return; // Static bodies do not move
    }

    public void OnCollision(IPhysicsBody other)
    {
        return; // Static bodies do not respond to collisions
    }

    public bool IsColliding(IPhysicsBody other)
    {
        // Ensure that the bodies are not already intersecting
        if (this.BoundingBoxSize.X <= 0 || this.BoundingBoxSize.Y <= 0 || this.BoundingBoxSize.Z <= 0 ||
            other.BoundingBoxSize.X <= 0 || other.BoundingBoxSize.Y <= 0 || other.BoundingBoxSize.Z <= 0)
        {
            throw new InvalidOperationException("Bounding box sizes must be greater than zero.");
        }
        Vector3 thisMin = Position - BoundingBoxSize / 2;
        Vector3 thisMax = Position + BoundingBoxSize / 2;
        Vector3 otherMin = other.Position - other.BoundingBoxSize / 2;
        Vector3 otherMax = other.Position + other.BoundingBoxSize / 2;
        
        return (thisMin.X <= otherMax.X && thisMax.X >= otherMin.X) &&
               (thisMin.Y <= otherMax.Y && thisMax.Y >= otherMin.Y) &&
               (thisMin.Z <= otherMax.Z && thisMax.Z >= otherMin.Z);
    }

    public bool IsStatic { get; }
    public bool IsKinematic { get; }
    public float Mass { get; set; }
}