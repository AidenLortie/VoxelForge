namespace VoxelForge.Shared.Physics.Math;

/// <summary>
/// Represents a 2D vector with X and Y components.
/// Provides common vector operations for 2D mathematics.
/// </summary>
public struct Vector2
{
    private float _x;
    private float _y;
    
    /// <summary>
    /// Gets or sets the X component of the vector.
    /// </summary>
    public float X
    {
        get => _x;
        set => _x = value;
    }

    /// <summary>
    /// Gets or sets the Y component of the vector.
    /// </summary>
    public float Y
    {
        get => _y;
        set => _y = value;
    }
    
    /// <summary>
    /// Initializes a new Vector2 with the specified X and Y components.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component (defaults to 0).</param>
    public Vector2(float x, float y = 0)
    {
        _x = x;
        _y = y;
    }
    
    /// <summary>Gets a Vector2 with both components set to 0.</summary>
    public static Vector2 Zero => new(0, 0);
    
    /// <summary>Gets a Vector2 with both components set to 1.</summary>
    public static Vector2 One => new(1, 1);
    
    /// <summary>Gets the unit vector pointing along the positive X axis (1, 0).</summary>
    public static Vector2 UnitX => new(1, 0);
    
    /// <summary>Gets the unit vector pointing along the positive Y axis (0, 1).</summary>
    public static Vector2 UnitY => new(0, 1);
    
    /// <summary>Gets the unit vector pointing up (0, 1).</summary>
    public static Vector2 Up => new(0, 1);
    
    /// <summary>Gets the unit vector pointing down (0, -1).</summary>
    public static Vector2 Down => new(0, -1);
    
    /// <summary>Gets the unit vector pointing left (-1, 0).</summary>
    public static Vector2 Left => new(-1, 0);
    
    /// <summary>Gets the unit vector pointing right (1, 0).</summary>
    public static Vector2 Right => new(1, 0);
    
    /// <summary>Adds two vectors component-wise.</summary>
    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
    
    /// <summary>Subtracts one vector from another component-wise.</summary>
    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
    
    /// <summary>Multiplies a vector by a scalar.</summary>
    public static Vector2 operator *(Vector2 a, float scalar) => new(a.X * scalar, a.Y * scalar);
    
    /// <summary>Divides a vector by a scalar.</summary>
    public static Vector2 operator /(Vector2 a, float scalar) => new(a.X / scalar, a.Y / scalar);
    
    /// <summary>Negates a vector (reverses its direction).</summary>
    public static Vector2 operator -(Vector2 a) => new(-a.X, -a.Y);
    
    /// <summary>Tests two vectors for equality.</summary>
    public static bool operator ==(Vector2 a, Vector2 b) => a.X == b.X && a.Y == b.Y;
    
    /// <summary>Tests two vectors for inequality.</summary>
    public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);
    
    /// <summary>Tests this vector for equality with another object.</summary>
    public override bool Equals(object? obj) => obj is Vector2 v && this == v;
    
    /// <summary>Gets the hash code for this vector.</summary>
    public override int GetHashCode() => HashCode.Combine(X, Y);
    
    /// <summary>Returns a string representation of this vector in the format "(X, Y)".</summary>
    public override string ToString() => $"({X}, {Y})";
    
    /// <summary>
    /// Calculates the length (magnitude) of this vector.
    /// </summary>
    /// <returns>The length of the vector.</returns>
    public float Length() => MathF.Sqrt((float)(X * X + Y * Y));
    
    /// <summary>
    /// Returns a normalized copy of this vector (length = 1).
    /// </summary>
    /// <returns>A unit vector in the same direction, or zero if this vector has zero length.</returns>
    public Vector2 Normalize()
    {
        var length = Length();
        if (length == 0) return new Vector2(0, 0);
        return new Vector2(X / length, Y / length);
    }
    
    /// <summary>
    /// Calculates the dot product with another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The dot product.</returns>
    public float Dot(Vector2 other) => X * other.X + Y * other.Y;
    
    /// <summary>
    /// Calculates the 2D cross product (Z component) with another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The Z component of the 3D cross product.</returns>
    public float Cross(Vector2 other) => X * other.Y - Y * other.X;
    
    /// <summary>
    /// Calculates the distance to another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The distance between the two vectors.</returns>
    public float Distance(Vector2 other) => (this - other).Length();
    
    /// <summary>
    /// Calculates the distance between two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The distance between the vectors.</returns>
    public static float Distance(Vector2 a, Vector2 b) => (a - b).Length();
    
    /// <summary>
    /// Linearly interpolates between this vector and a target vector.
    /// </summary>
    /// <param name="target">The target vector.</param>
    /// <param name="t">The interpolation factor (0 = this, 1 = target).</param>
    /// <returns>The interpolated vector.</returns>
    public Vector2 Lerp(Vector2 target, float t) => this + (target - this) * t;
    
    /// <summary>
    /// Linearly interpolates between two vectors.
    /// </summary>
    /// <param name="a">The start vector.</param>
    /// <param name="b">The end vector.</param>
    /// <param name="t">The interpolation factor (0 = a, 1 = b).</param>
    /// <returns>The interpolated vector.</returns>
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => a + (b - a) * t;
    
    /// <summary>
    /// Clamps this vector's components between minimum and maximum values.
    /// </summary>
    /// <param name="min">The minimum values.</param>
    /// <param name="max">The maximum values.</param>
    /// <returns>The clamped vector.</returns>
    public Vector2 Clamp(Vector2 min, Vector2 max) => new(
        MathF.Max((float)min.X, MathF.Min((float)max.X, (float)X)),
        MathF.Max((float)min.Y, MathF.Min((float)max.Y, (float)Y))
    );
    
    /// <summary>
    /// Clamps a vector's components between minimum and maximum values.
    /// </summary>
    /// <param name="value">The vector to clamp.</param>
    /// <param name="min">The minimum values.</param>
    /// <param name="max">The maximum values.</param>
    /// <returns>The clamped vector.</returns>
    public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) => new(
        MathF.Max((float)min.X, MathF.Min((float)max.X, (float)value.X)),
        MathF.Max((float)min.Y, MathF.Min((float)max.Y, (float)value.Y))
    );
    
    /// <summary>
    /// Converts this Vector2 to a Vector3.
    /// </summary>
    /// <param name="z">The Z component for the Vector3 (defaults to 0).</param>
    /// <returns>A new Vector3 with this vector's X and Y components.</returns>
    public Vector3 ToVector3(float z = 0) => new(X, Y, z);
    
    /// <summary>
    /// Creates a unit vector from an angle in radians.
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <returns>A unit vector pointing in the specified direction.</returns>
    public static Vector2 FromAngle(float angle)
    {
        return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }
    
    /// <summary>
    /// Calculates the angle of this vector in radians from the positive X axis.
    /// </summary>
    /// <returns>The angle in radians.</returns>
    public float Angle() => MathF.Atan2((float)Y, (float)X);
    
    /// <summary>
    /// Calculates the angle between two vectors in radians.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The angle between the vectors in radians.</returns>
    public static float AngleBetween(Vector2 a, Vector2 b)
    {
        return MathF.Atan2((float)(a.X * b.Y - a.Y * b.X), (float)(a.X * b.X + a.Y * b.Y));
    }
    
    
    
}