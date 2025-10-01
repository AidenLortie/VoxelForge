namespace VoxelForge.Shared.Physics;

public class PhysicsEngine
{
    Mesh[] _meshes; // All meshes in the physics engine
    
    public PhysicsEngine()
    {
        _meshes = Array.Empty<Mesh>();
    }
    
    public void AddMesh(Mesh mesh)
    {
        Array.Resize(ref _meshes, _meshes.Length + 1);
        _meshes[^1] = mesh;
    }
    
    public Mesh[] GetMeshes() => _meshes;
    
    public void Tick(float deltaTime)
    {
        foreach (var mesh in _meshes)
        {
            // Update all physics bodies associated with this mesh
            foreach (var body in mesh.GetPhysicsBodies)
            {
                body.Update(deltaTime, this);
                
                if (!body.IsStatic)
                {
                    // Simple gravity application for non-static bodies
                    body.ApplyForce(new Math.Vector3(0, -9.81f * body.Mass, 0));
                    
                    // Check for collisions with other bodies in the engine
                    foreach (var otherMesh in _meshes)
                    {
                        if (otherMesh == mesh) continue; // Skip self
                        
                        foreach (var otherBody in otherMesh.GetPhysicsBodies)
                        {
                            if (body != otherBody && body.IsColliding(otherBody))
                            {
                                body.OnCollision(otherBody);
                                otherBody.OnCollision(body);
                            }
                        }
                    }
                }
            }
        }
    }
}