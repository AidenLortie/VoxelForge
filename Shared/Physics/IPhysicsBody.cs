using VoxelForge.Shared.Physics.Math;

namespace VoxelForge.Shared.Physics;

public interface IPhysicsBody
{
    Vector3 Position { get; set; }
    Vector3 Velocity { get; set; }
    Quaternion Rotation { get; set; }
    Vector3 BoundingBoxSize { get; set; }
    
    void ApplyForce(Vector3 force);
    void ApplyImpulse(Vector3 impulse);
    void Update(float deltaTime, PhysicsEngine engine);
    void OnCollision(IPhysicsBody other);
    bool IsColliding(IPhysicsBody other);
    
    bool IsStatic { get; }
    bool IsKinematic { get; }
    float Mass { get; set; }
}