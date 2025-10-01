namespace VoxelForge.Shared.Physics.Math;

public struct Vector2
{
    private float _x;
    private float _y;
    
    public float X
    {
        get => _x;
        set => _x = value;
    }

    public float Y
    {
        get => _y;
        set => _y = value;
    }
    
    public Vector2(float x, float y = 0)
    {
        _x = x;
        _y = y;
    }
    
    public static Vector2 Zero => new(0, 0);
    public static Vector2 One => new(1, 1);
    public static Vector2 UnitX => new(1, 0);
    public static Vector2 UnitY => new(0, 1);
    public static Vector2 Up => new(0, 1);
    public static Vector2 Down => new(0, -1);
    public static Vector2 Left => new(-1, 0);
    public static Vector2 Right => new(1, 0);
    
    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vector2 operator *(Vector2 a, float scalar) => new(a.X * scalar, a.Y * scalar);
    public static Vector2 operator /(Vector2 a, float scalar) => new(a.X / scalar, a.Y / scalar);
    public static Vector2 operator -(Vector2 a) => new(-a.X, -a.Y);
    public static bool operator ==(Vector2 a, Vector2 b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);
    public override bool Equals(object? obj) => obj is Vector2 v && this == v;
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public override string ToString() => $"({X}, {Y})";
    public float Length() => MathF.Sqrt((float)(X * X + Y * Y));
    public Vector2 Normalize()
    {
        var length = Length();
        if (length == 0) return new Vector2(0, 0);
        return new Vector2(X / length, Y / length);
    }
    public float Dot(Vector2 other) => X * other.X + Y * other.Y;
    public float Cross(Vector2 other) => X * other.Y - Y * other.X;
    public float Distance(Vector2 other) => (this - other).Length();
    public static float Distance(Vector2 a, Vector2 b) => (a - b).Length();
    public Vector2 Lerp(Vector2 target, float t) => this + (target - this) * t;
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => a + (b - a) * t;
    
    public Vector2 Clamp(Vector2 min, Vector2 max) => new(
        MathF.Max((float)min.X, MathF.Min((float)max.X, (float)X)),
        MathF.Max((float)min.Y, MathF.Min((float)max.Y, (float)Y))
    );
    
    public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) => new(
        MathF.Max((float)min.X, MathF.Min((float)max.X, (float)value.X)),
        MathF.Max((float)min.Y, MathF.Min((float)max.Y, (float)value.Y))
    );
    
    public Vector3 ToVector3(float z = 0) => new(X, Y, z);
    
    public static Vector2 FromAngle(float angle)
    {
        return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }
    
    public float Angle() => MathF.Atan2((float)Y, (float)X);
    
    public static float AngleBetween(Vector2 a, Vector2 b)
    {
        return MathF.Atan2((float)(a.X * b.Y - a.Y * b.X), (float)(a.X * b.X + a.Y * b.Y));
    }
    
    
    
}