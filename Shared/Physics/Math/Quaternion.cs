using System.Numerics;

namespace VoxelForge.Shared.Physics.Math;

public struct Quaternion
{
    public float W; // Real part
    public float X; // i component
    public float Y; // j component
    public float Z; // k component
    
    public Quaternion(float w, float x, float y, float z)
    {
        W = w;
        X = x;
        Y = y;
        Z = z;
    }

    public Quaternion Normalize()
    {
        float mag = (float)System.Math.Sqrt(W * W + X * X + Y * Y + Z * Z);
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
    
    public static Quaternion FromAxisAngle(Vector3 axis, float angle)
    {
        var halfAngle = angle / 2;
        var s = (float)System.Math.Sin(halfAngle);
        return new Quaternion((float)System.Math.Cos(halfAngle), axis.X * s, axis.Y * s, axis.Z * s).Normalize();
    }
    
    public Vector3 Rotate(Vector3 v)
    {
        var qv = new Quaternion(0, v.X, v.Y, v.Z);
        var result = this * qv * Conjugate().Normalize();
        return new Vector3(result.X, result.Y, result.Z);
    }
    
    public Quaternion Conjugate() => new Quaternion(W, -X, -Y, -Z);

}