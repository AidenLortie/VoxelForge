namespace VoxelForge.Shared.Physics;

/// <summary>
/// Main physics simulation engine for VoxelForge.
/// Manages physics bodies, applies forces, and handles collision detection.
/// </summary>
public class PhysicsEngine
{
    Mesh[] _meshes; // All meshes in the physics engine
    
    /// <summary>
    /// Initializes a new PhysicsEngine with no meshes.
    /// </summary>
    public PhysicsEngine()
    {
        _meshes = Array.Empty<Mesh>();
    }
    
    /// <summary>
    /// Adds a mesh with physics bodies to the physics engine.
    /// </summary>
    /// <param name="mesh">The mesh to add.</param>
    public void AddMesh(Mesh mesh)
    {
        Array.Resize(ref _meshes, _meshes.Length + 1);
        _meshes[^1] = mesh;
    }
    
    /// <summary>
    /// Gets all meshes currently registered in the physics engine.
    /// </summary>
    /// <returns>An array of all meshes.</returns>
    public Mesh[] GetMeshes() => _meshes;
    
    /// <summary>
    /// Updates the physics simulation by one tick.
    /// Applies forces, updates physics bodies, and handles collisions.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last tick in seconds.</param>
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