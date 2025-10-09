using OpenTK.Mathematics;
using VoxelForge.Shared.World;

namespace VoxelForge.Client.Player;

/// <summary>
/// Manages player physics and collision detection.
/// </summary>
public class PlayerController
{
    private Vector3 _position;
    private Vector3 _velocity;
    private readonly World _world;
    
    // Player dimensions (AABB)
    private const float PlayerWidth = 0.6f;
    private const float PlayerHeight = 1.8f;
    private const float PlayerDepth = 0.6f;
    
    // Physics constants
    private const float Gravity = -20.0f;
    private const float JumpVelocity = 8.0f;
    private const float TerminalVelocity = -50.0f;
    
    /// <summary>
    /// Gets or sets the player's position.
    /// </summary>
    public Vector3 Position
    {
        get => _position;
        set => _position = value;
    }
    
    /// <summary>
    /// Gets the player's velocity.
    /// </summary>
    public Vector3 Velocity => _velocity;
    
    /// <summary>
    /// Gets whether the player is on the ground.
    /// </summary>
    public bool IsOnGround { get; private set; }
    
    /// <summary>
    /// Gets or sets whether gravity is enabled for this player.
    /// </summary>
    public bool GravityEnabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether collision detection is enabled.
    /// </summary>
    public bool CollisionEnabled { get; set; } = true;
    
    /// <summary>
    /// Creates a new PlayerController.
    /// </summary>
    /// <param name="world">The world for collision detection</param>
    /// <param name="spawnPosition">Initial spawn position</param>
    public PlayerController(World world, Vector3 spawnPosition)
    {
        _world = world;
        _position = spawnPosition;
        _velocity = Vector3.Zero;
    }
    
    /// <summary>
    /// Updates player physics.
    /// </summary>
    /// <param name="deltaTime">Time since last update in seconds</param>
    public void Update(float deltaTime)
    {
        if (GravityEnabled)
        {
            // Apply gravity
            _velocity.Y += Gravity * deltaTime;
            
            // Clamp to terminal velocity
            _velocity.Y = Math.Max(_velocity.Y, TerminalVelocity);
        }
        
        // Apply velocity
        Vector3 movement = _velocity * deltaTime;
        
        if (CollisionEnabled)
        {
            // Apply collision detection
            movement = ApplyCollision(movement);
        }
        
        _position += movement;
        
        // Decay horizontal velocity (friction)
        _velocity.X *= 0.8f;
        _velocity.Z *= 0.8f;
        
        // Stop small velocities
        if (Math.Abs(_velocity.X) < 0.01f) _velocity.X = 0;
        if (Math.Abs(_velocity.Z) < 0.01f) _velocity.Z = 0;
    }
    
    /// <summary>
    /// Moves the player in a direction.
    /// </summary>
    /// <param name="direction">Direction to move (should be normalized)</param>
    /// <param name="speed">Movement speed</param>
    public void Move(Vector3 direction, float speed)
    {
        if (IsOnGround)
        {
            _velocity.X += direction.X * speed;
            _velocity.Z += direction.Z * speed;
            
            // Clamp horizontal speed
            float horizontalSpeed = new Vector2(_velocity.X, _velocity.Z).Length;
            if (horizontalSpeed > speed)
            {
                float scale = speed / horizontalSpeed;
                _velocity.X *= scale;
                _velocity.Z *= scale;
            }
        }
    }
    
    /// <summary>
    /// Makes the player jump if on ground.
    /// </summary>
    public void Jump()
    {
        if (IsOnGround && GravityEnabled)
        {
            _velocity.Y = JumpVelocity;
            IsOnGround = false;
        }
    }
    
    /// <summary>
    /// Applies collision detection to movement.
    /// </summary>
    private Vector3 ApplyCollision(Vector3 movement)
    {
        IsOnGround = false;
        
        // Get player AABB
        Vector3 min = _position - new Vector3(PlayerWidth / 2, 0, PlayerDepth / 2);
        Vector3 max = _position + new Vector3(PlayerWidth / 2, PlayerHeight, PlayerDepth / 2);
        
        // Apply movement in each axis separately
        Vector3 result = movement;
        
        // Y axis (vertical) - check ceiling and floor
        if (movement.Y != 0)
        {
            Vector3 testMin = min + new Vector3(0, movement.Y, 0);
            Vector3 testMax = max + new Vector3(0, movement.Y, 0);
            
            if (CheckAABBCollision(testMin, testMax))
            {
                result.Y = 0;
                _velocity.Y = 0;
                
                // Check if we're on ground (moving down and colliding)
                if (movement.Y < 0)
                {
                    IsOnGround = true;
                }
            }
        }
        
        // X axis (horizontal)
        if (movement.X != 0)
        {
            Vector3 testMin = min + new Vector3(result.X, result.Y, 0);
            Vector3 testMax = max + new Vector3(result.X, result.Y, 0);
            
            if (CheckAABBCollision(testMin, testMax))
            {
                result.X = 0;
                _velocity.X = 0;
            }
        }
        
        // Z axis (horizontal)
        if (movement.Z != 0)
        {
            Vector3 testMin = min + new Vector3(result.X, result.Y, result.Z);
            Vector3 testMax = max + new Vector3(result.X, result.Y, result.Z);
            
            if (CheckAABBCollision(testMin, testMax))
            {
                result.Z = 0;
                _velocity.Z = 0;
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Checks if an AABB collides with any blocks in the world.
    /// </summary>
    private bool CheckAABBCollision(Vector3 min, Vector3 max)
    {
        // Get block coordinates to check
        int minX = (int)Math.Floor(min.X);
        int minY = (int)Math.Floor(min.Y);
        int minZ = (int)Math.Floor(min.Z);
        int maxX = (int)Math.Floor(max.X);
        int maxY = (int)Math.Floor(max.Y);
        int maxZ = (int)Math.Floor(max.Z);
        
        // Check all blocks in range
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (IsBlockSolid(x, y, z))
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Checks if a block at the given world coordinates is solid.
    /// </summary>
    private bool IsBlockSolid(int x, int y, int z)
    {
        // Out of bounds checks
        if (y < 0 || y >= 256)
            return y < 0; // Below world is solid, above is not
        
        // Get chunk coordinates
        int chunkX = x / 16;
        int chunkZ = z / 16;
        
        // Adjust for negative coordinates
        if (x < 0) chunkX--;
        if (z < 0) chunkZ--;
        
        // Check chunk bounds
        if (chunkX < 0 || chunkX >= 16 || chunkZ < 0 || chunkZ >= 16)
            return false; // Outside world bounds
        
        var chunk = _world.GetChunk(chunkX, 0, chunkZ);
        if (chunk == null)
            return false;
        
        // Get local coordinates
        int localX = x - chunkX * 16;
        int localZ = z - chunkZ * 16;
        
        // Ensure local coordinates are positive
        while (localX < 0) localX += 16;
        while (localZ < 0) localZ += 16;
        localX %= 16;
        localZ %= 16;
        
        ushort blockId = chunk.GetBlockStateId(localX, y, localZ);
        
        // Air (id 0) is not solid
        return blockId != 0;
    }
}
