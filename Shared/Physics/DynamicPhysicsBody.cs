using VoxelForge.Shared.Physics.Math;

namespace VoxelForge.Shared.Physics;

public class DynamicPhysicsBody: IPhysicsBody
{
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 BoundingBoxSize { get; set; }
    
    public void ApplyForce(Vector3 force)
    {
        this.Velocity += force;
    }

    public void ApplyImpulse(Vector3 impulse)
    {
        this.Velocity += impulse / Mass;
    }

    public void Update(float deltaTime, PhysicsEngine engine)
    {
        Position += Velocity * deltaTime;
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

    public void OnCollision(IPhysicsBody other)
    {
        // If static - snap in direction of least penetration
        if (other.IsStatic)
        {
            Vector3 thisMin = Position - BoundingBoxSize / 2;
            Vector3 thisMax = Position + BoundingBoxSize / 2;
            Vector3 otherMin = other.Position - other.BoundingBoxSize / 2;
            Vector3 otherMax = other.Position + other.BoundingBoxSize / 2;
            float overlapX = MathF.Min((float)thisMax.X, (float)otherMax.X) -
                             MathF.Max((float)thisMin.X, (float)otherMin.X);
            float overlapY = MathF.Min((float)thisMax.Y, (float)otherMax.Y) -
                             MathF.Max((float)thisMin.Y, (float)otherMin.Y);
            float overlapZ = MathF.Min((float)thisMax.Z, (float)otherMax.Z) -
                             MathF.Max((float)thisMin.Z, (float)otherMin.Z);

            // Find the axis of least penetration
            if (overlapX < overlapY && overlapX < overlapZ)
            {
                // Move along X axis
                if (Position.X < other.Position.X)
                {
                    Position = new Vector3(Position.X - overlapX, Position.Y, Position.Z);
                    Velocity = new Vector3(0, Velocity.Y, Velocity.Z);
                }
                else
                {
                    Position = new Vector3(Position.X + overlapX, Position.Y, Position.Z);
                    Velocity = new Vector3(0, Velocity.Y, Velocity.Z);
                }
            }
            else if (overlapY < overlapX && overlapY < overlapZ)
            {
                // Move along Y axis
                if (Position.Y < other.Position.Y)
                {
                    Position = new Vector3(Position.X, Position.Y - overlapY, Position.Z);
                    Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
                }
                else
                {
                    Position = new Vector3(Position.X, Position.Y + overlapY, Position.Z);
                    Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
                }
            }
            else
            {
                // Move along Z axis
                if (Position.Z < other.Position.Z)
                {
                    Position = new Vector3(Position.X, Position.Y, Position.Z - overlapZ);
                    Velocity = new Vector3(Velocity.X, Velocity.Y, 0);
                }
                else
                {
                    Position = new Vector3(Position.X, Position.Y, Position.Z + overlapZ);
                    Velocity = new Vector3(Velocity.X, Velocity.Y, 0);
                }
            }
        }
    }

    public bool IsStatic { get; }
    public bool IsKinematic { get; }
    public float Mass { get; set; }
}