namespace VoxelForge.Shared.Physics.Math;

public struct Quaternion
{
    public double W; // Real part
    public double X; // i component
    public double Y; // j component
    public double Z; // k component
    
    public Quaternion(double w, double x, double y, double z)
    {
        W = w;
        X = x;
        Y = y;
        Z = z;
    }

    public Quaternion Normalize()
    {
        double mag = System.Math.Sqrt(W * W + X * X + Y * Y + Z * Z);
        if (mag == 0) return new Quaternion(1, 0, 0, 0); // Return identity quaternion if magnitude is zero
        return new Quaternion(W / mag, X / mag, Y / mag, Z / mag);
    }
    
    public static Quaternion operator *(Quaternion q1, Quaternion q2)
    {
        return new Quaternion(
            q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z,
            q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y,
            q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X,
            q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W
        );
    }
    
    public override string ToString() => $"({W}, {X}, {Y}, {Z})";
    
    public static Quaternion FromAxisAngle(Vector3 axis, double angle)
    {
        var halfAngle = angle / 2;
        var s = System.Math.Sin(halfAngle);
        return new Quaternion(System.Math.Cos(halfAngle), axis.X * s, axis.Y * s, axis.Z * s).Normalize();
    }
    
    public Vector3 Rotate(Vector3 v)
    {
        var qv = new Quaternion(0, v.X, v.Y, v.Z);
        var result = this * qv * Conjugate().Normalize();
        return new Vector3(result.X, result.Y, result.Z);
    }
    
    public Quaternion Conjugate() => new Quaternion(W, -X, -Y, -Z);
}