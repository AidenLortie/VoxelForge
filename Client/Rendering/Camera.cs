using OpenTK.Mathematics;

namespace VoxelForge.Client.Rendering;

/// <summary>
/// Represents a camera with view and projection matrices.
/// Supports first-person camera movement.
/// </summary>
public class Camera
{
    private Vector3 _position;
    private float _pitch; // Rotation around X axis (up/down)
    private float _yaw;   // Rotation around Y axis (left/right)
    
    // /// Camera position in world space
    public Vector3 Position
    {
        get => _position;
        set => _position = value;
    }
    
    // /// Pitch angle in radians
    public float Pitch
    {
        get => _pitch;
        set => _pitch = Math.Clamp(value, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);
    }
    
    // /// Yaw angle in radians
    public float Yaw
    {
        get => _yaw;
        set => _yaw = value;
    }
    
    // /// Field of view in degrees
    public float Fov { get; set; } = 70.0f;
    
    // /// Aspect ratio (width / height)
    public float AspectRatio { get; set; } = 16.0f / 9.0f;
    
    // /// Near clipping plane
    public float Near { get; set; } = 0.1f;
    
    // /// Far clipping plane
    public float Far { get; set; } = 1000.0f;
    
    // /// Movement speed in units per second
    public float MovementSpeed { get; set; } = 5.0f;
    
    // /// Mouse sensitivity
    public float MouseSensitivity { get; set; } = 0.002f;
    
    // /// Creates a camera at the origin looking down the -Z axis.
    public Camera()
    {
        _position = Vector3.Zero;
        _pitch = 0;
        _yaw = -MathHelper.PiOver2; // Look down -Z axis
    }
    
    // /// Creates a camera at the specified position.
    public Camera(Vector3 position) : this()
    {
        _position = position;
    }
    
    // /// Gets the forward direction vector.
    public Vector3 Forward
    {
        get
        {
            return new Vector3(
                (float)(Math.Cos(_pitch) * Math.Cos(_yaw)),
                (float)Math.Sin(_pitch),
                (float)(Math.Cos(_pitch) * Math.Sin(_yaw))
            );
        }
    }
    
    // /// Gets the right direction vector.
    public Vector3 Right
    {
        get
        {
            return Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
        }
    }
    
    // /// Gets the up direction vector.
    public Vector3 Up
    {
        get
        {
            return Vector3.Normalize(Vector3.Cross(Right, Forward));
        }
    }
    
    // /// Calculates the view matrix for this camera.
    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(_position, _position + Forward, Vector3.UnitY);
    }
    
    // /// Calculates the projection matrix for this camera.
    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(Fov),
            AspectRatio,
            Near,
            Far
        );
    }
    
    // /// Moves the camera forward/backward relative to its current orientation.
    public void MoveForward(float amount)
    {
        _position += Forward * amount;
    }
    
    // /// Moves the camera right/left relative to its current orientation.
    public void MoveRight(float amount)
    {
        _position += Right * amount;
    }
    
    // /// Moves the camera up/down in world space.
    public void MoveUp(float amount)
    {
        _position.Y += amount;
    }
    
    // /// Rotates the camera based on mouse movement.
    public void Rotate(float deltaX, float deltaY)
    {
        Yaw += deltaX * MouseSensitivity;
        Pitch -= deltaY * MouseSensitivity;
    }
}
