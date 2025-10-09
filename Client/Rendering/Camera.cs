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
    
    /// <summary>
    /// Camera position in world space
    /// </summary>
    public Vector3 Position
    {
        get => _position;
        set => _position = value;
    }
    
    /// <summary>
    /// Pitch angle in radians
    /// </summary>
    public float Pitch
    {
        get => _pitch;
        set => _pitch = Math.Clamp(value, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);
    }
    
    /// <summary>
    /// Yaw angle in radians
    /// </summary>
    public float Yaw
    {
        get => _yaw;
        set => _yaw = value;
    }
    
    /// <summary>
    /// Field of view in degrees
    /// </summary>
    public float Fov { get; set; } = 70.0f;
    
    /// <summary>
    /// Aspect ratio (width / height)
    /// </summary>
    public float AspectRatio { get; set; } = 16.0f / 9.0f;
    
    /// <summary>
    /// Near clipping plane
    /// </summary>
    public float Near { get; set; } = 0.1f;
    
    /// <summary>
    /// Far clipping plane
    /// </summary>
    public float Far { get; set; } = 1000.0f;
    
    /// <summary>
    /// Movement speed in units per second
    /// </summary>
    public float MovementSpeed { get; set; } = 5.0f;
    
    /// <summary>
    /// Mouse sensitivity
    /// </summary>
    public float MouseSensitivity { get; set; } = 0.002f;
    
    /// <summary>
    /// Creates a camera at the origin looking down the -Z axis.
    /// </summary>
    public Camera()
    {
        _position = Vector3.Zero;
        _pitch = 0;
        _yaw = -MathHelper.PiOver2; // Look down -Z axis
    }
    
    /// <summary>
    /// Creates a camera at the specified position.
    /// </summary>
    public Camera(Vector3 position) : this()
    {
        _position = position;
    }
    
    /// <summary>
    /// Gets the forward direction vector.
    /// </summary>
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
    
    /// <summary>
    /// Gets the right direction vector.
    /// </summary>
    public Vector3 Right
    {
        get
        {
            return Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
        }
    }
    
    /// <summary>
    /// Gets the up direction vector.
    /// </summary>
    public Vector3 Up
    {
        get
        {
            return Vector3.Normalize(Vector3.Cross(Right, Forward));
        }
    }
    
    /// <summary>
    /// Calculates the view matrix for this camera.
    /// </summary>
    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(_position, _position + Forward, Vector3.UnitY);
    }
    
    /// <summary>
    /// Calculates the projection matrix for this camera.
    /// </summary>
    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(Fov),
            AspectRatio,
            Near,
            Far
        );
    }
    
    /// <summary>
    /// Moves the camera forward/backward relative to its current orientation.
    /// </summary>
    public void MoveForward(float amount)
    {
        _position += Forward * amount;
    }
    
    /// <summary>
    /// Moves the camera right/left relative to its current orientation.
    /// </summary>
    public void MoveRight(float amount)
    {
        _position += Right * amount;
    }
    
    /// <summary>
    /// Moves the camera up/down in world space.
    /// </summary>
    public void MoveUp(float amount)
    {
        _position.Y += amount;
    }
    
    /// <summary>
    /// Rotates the camera based on mouse movement.
    /// </summary>
    public void Rotate(float deltaX, float deltaY)
    {
        Yaw += deltaX * MouseSensitivity;
        Pitch -= deltaY * MouseSensitivity;
    }
}
