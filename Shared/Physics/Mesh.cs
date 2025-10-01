using VoxelForge.Shared.Physics.Math;

namespace VoxelForge.Shared.Physics;

public class Mesh
{
    // Mesh data should be provided as quads
    private Vector3[] _vertices;
    private int[] _indices;
    
    IPhysicsBody[] _physicsBodies; // One or more physics bodies associated with this mesh
    
    public Vector3[] Vertices => _vertices;
    public int[] Indices => _indices;
    
    public Mesh(Vector3[] vertices, int[] indices)
    {
        _vertices = vertices;
        _indices = indices;
    }
    
    public void calculatePhysicsBodies()
    {
        // Greedy algorithm to create axis-aligned bounding boxes (AABBs) around quads from voxel vertices
        List<IPhysicsBody> bodies = new List<IPhysicsBody>();
        HashSet<int> processedQuads = new HashSet<int>();
        
        
    }
    
    public void updatePhysicsBodies(float deltaTime, PhysicsEngine engine)
    {
        foreach (var body in _physicsBodies)
        {
            body.Update(deltaTime, engine);
        }
    }
    
    public IPhysicsBody[] GetPhysicsBodies => _physicsBodies;
}