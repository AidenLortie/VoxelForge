namespace VoxelForge.Shared.Physics.Math;

public struct Vector3
{
    private double x;
    private double y;
    private double z;
    
    // getters and setters
    public double X
    { get => x; set => x = value; }
    public double Y
    { get => y; set => y = value; }
    public double Z
    { get => z; set => z = value; }
    
    // constructor
    public Vector3(double x, double y = 0, double z = 0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public Vector3(double x, Vector2 yz) : this(x, yz.X, yz.Y) { }
    public Vector3(Vector2 xy, double z) : this(xy.X, xy.Y, z) { }
    public Vector3(Vector2 xy) : this(xy.X, xy.Y, 0) { }
    public static Vector3 Zero => new(0, 0, 0);
    public static Vector3 One => new(1, 1, 1);
    public static Vector3 UnitX => new(1, 0, 0);
    public static Vector3 UnitY => new(0, 1, 0);
    public static Vector3 UnitZ => new(0, 0, 1);
    public static Vector3 Up => new(0, 1, 0);
    public static Vector3 Down => new(0, -1, 0);
    public static Vector3 Left => new(-1, 0, 0);
    public static Vector3 Right => new(1, 0, 0);
    public static Vector3 Forward => new(0, 0, 1);
    public static Vector3 Backward => new(0, 0, -1);
    
    public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3 operator *(Vector3 a, double scalar) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);
    public static Vector3 operator /(Vector3 a, double scalar) => new(a.X / scalar, a.Y / scalar, a.Z / scalar);
    public static Vector3 operator -(Vector3 a) => new(-a.X, -a.Y, -a.Z);
    public static bool operator ==(Vector3 a, Vector3 b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);
    public override bool Equals(object obj) => obj is Vector3 v && this == v;
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    public override string ToString() => $"({X}, {Y}, {Z})";
    public double Length() => System.Math.Sqrt(X * X + Y * Y + Z * Z);

    public Vector3 Normalize()
    {
        var length = Length();
        if (length == 0) return new Vector3(0, 0, 0);
        return new Vector3(X / length, Y / length, Z / length);
    }
    public double Dot(Vector3 other) => X * other.X + Y * other.Y + Z * other.Z;
    public Vector3 Cross(Vector3 other) => new(
        Y * other.Z - Z * other.Y,
        Z * other.X - X * other.Z,
        X * other.Y - Y * other.X
    );
    public Vector2 ToVector2() => new(X, Y);
    public Vector2 ToVector2XZ() => new(X, Z);
    public Vector2 ToVector2YZ() => new(Y, Z);
    
    public static double Distance(Vector3 a, Vector3 b) => (a - b).Length();
    public static double DistanceSquared(Vector3 a, Vector3 b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return dx * dx + dy * dy + dz * dz;
    }
    public Vector3 Clamp(Vector3 min, Vector3 max) => new(
        System.Math.Max(min.X, System.Math.Min(max.X, X)),
        System.Math.Max(min.Y, System.Math.Min(max.Y, Y)),
        System.Math.Max(min.Z, System.Math.Min(max.Z, Z))
    );
    public Vector3 Lerp(Vector3 target, double t) => this + (target - this) * t;
    public Vector3 Reflect(Vector3 normal)
    {
        var dot = this.Dot(normal);
        return this - normal * (2 * dot);
    }
    public Vector3 Abs() => new(System.Math.Abs(X), System.Math.Abs(Y), System.Math.Abs(Z));
    public Vector3 Floor() => new(System.Math.Floor(X), System.Math.Floor(Y), System.Math.Floor(Z));
    public Vector3 Ceiling() => new(System.Math.Ceiling(X), System.Math.Ceiling(Y), System.Math.Ceiling(Z));
    public Vector3 Round() => new(System.Math.Round(X), System.Math.Round(Y), System.Math.Round(Z));
    public Vector3 Min(Vector3 other) => new(System.Math.Min(X, other.X), System.Math.Min(Y, other.Y), System.Math.Min(Z, other.Z));
    public Vector3 Max(Vector3 other) => new(System.Math.Max(X, other.X), System.Math.Max(Y, other.Y), System.Math.Max(Z, other.Z));
    public Vector3 WithX(double x) => new(x, Y, Z);
    public Vector3 WithY(double y) => new(X, y, Z);
    public Vector3 WithZ(double z) => new(X, Y, z);
    
    public bool IsZero(double tolerance = 1e-10) => Length() < tolerance;

}