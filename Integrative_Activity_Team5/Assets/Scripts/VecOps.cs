using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class VecOps
{
    public static float DotProduct(Vector3 a, Vector3 b)
    {
        return Vector3.Dot(a, b);
    }

    public static Vector3 CrossProduct(Vector3 a, Vector3 b)
    {
        return Vector3.Cross(a, b);
    }

    public static float Magnitude(Vector3 v)
    {
        return v.magnitude;
    }

    public static Vector3 Normalize(Vector3 v)
    {
        return v.normalized;
    }

    public static float Angle(Vector3 from, Vector3 to)
    {
        return Vector3.Angle(from, to);
    }

    public static Matrix4x4 TranslateM(Vector3 t)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 3] = t.x;
        m[1, 3] = t.y;
        m[2, 3] = t.z;
        return m;
    }

    public static Matrix4x4 ScaleM(Vector3 scale)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = scale.x;
        m[1, 1] = scale.y;
        m[2, 2] = scale.z;
        return m;
    }

    public static Matrix4x4 RotateXM(float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        Matrix4x4 m = Matrix4x4.identity;
        m[1, 1] = cos;
        m[1, 2] = -sin;
        m[2, 1] = sin;
        m[2, 2] = cos;
        return m;
    }

    public static Matrix4x4 RotateYM(float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = cos;
        m[0, 2] = sin;
        m[2, 0] = -sin;
        m[2, 2] = cos;
        return m;
    }

    public static Matrix4x4 RotateZM(float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = cos;
        m[0, 1] = -sin;
        m[1, 0] = sin;
        m[1, 1] = cos;
        return m;
    }

    public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
    {
        Vector3 fromCrossTo = CrossProduct(from, to);
        float angle = Angle(from, to);
        return DotProduct(fromCrossTo, axis) < 0 ? -angle : angle;
    }

    public static List<Vector3> ApplyTransform(List<Vector3> originals, Matrix4x4 m)
    {
        List<Vector3> result = new List<Vector3>();
        foreach (Vector3 v in originals)
        {
            Vector4 temp = new Vector4(v.x, v.y, v.z, 1); 
            Vector4 transformed = m * temp;
            result.Add(new Vector3(transformed.x / transformed.w, transformed.y / transformed.w, transformed.z / transformed.w));
        }
        return result;
    }

    public static float Distance(Vector3 a, Vector3 b)
    {
        return Magnitude(b - a);
    }

    public static Vector3 Direction(Vector3 from, Vector3 to)
    {
        return Normalize(to - from);
    }
}