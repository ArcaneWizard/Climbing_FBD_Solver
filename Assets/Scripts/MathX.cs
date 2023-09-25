using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class MathX
{
    public static float SMALL_EPSILON = 0.00001f;

    /// <summary>
    /// returns true if two floats are equal (difference below 0.00001)
    /// </summary>
    public static bool Equal(float x, float y) 
    {
        if (x == y)
            return true;

        return Mathf.Abs(x - y) < SMALL_EPSILON;
    }

    /// <summary>
    /// returns 1 if x > y, -1 if x < y, and 0 if x equals y (difference below 0.00001)
    /// </summary>
    public static int Compare(float x, float y) {
        if (Equal(x,y))
            return 0;

        return (x > y) ? 1 : -1;
    }

    /// <summary>
    /// always returns a non-negative remainder
    /// </summary>
    public static int Modulo(int a, int b) => ((a % b) + b) % b;

    /// <summary>
    /// returns whether a/b = c/d. Will return true if both ratios are undefined
    /// </summary>
    public static bool IsEqualRatio(float a, float b, float c, float d)
    {
        if (Equal(b, 0) || Equal(d, 0))
            return Equal(b, d);

        return Equal(a / b, c / d);
    }

    /// <summary>
    /// returns intersection of two lines
    /// </summary>
    public static Vector2 GetIntersectionOfTwoLines(Vector2 line1Slope, Vector2 line2Slope, Vector2 line1Point, Vector2 line2Point)
    {
        Matrix4x4 a = Matrix4x4.identity;
        a[0, 0] = -line1Slope.y;
        a[0, 1] = line1Slope.x;
        a[1, 0] = -line2Slope.y;
        a[1, 1] = line2Slope.x;
        Vector4 b = new Vector4(-line1Point.x * line1Slope.y + line1Point.y * line1Slope.x, -line2Point.x * line2Slope.y + line2Point.y * line2Slope.x);
        var solution = a.inverse * b;

        return new Vector2(solution.x, solution.y);
    }

    /// <summary>
    /// returns whether the specified point lies on the line segment between two specified endpoints
    /// </summary>
    public static bool IsPointOnLineSegment2D(Vector2 point, Vector2 endPoint1, Vector2 endPoint2)
    {
        float x1 = point.x - endPoint1.x;
        float y1 = point.y - endPoint1.y;
        float x2 = point.x - endPoint2.x;
        float y2 = point.y - endPoint2.y;

        return IsEqualRatio(x1, y1, x2, y2);
    }

    /// <summary>
    /// returns whether the specified point lies on the line segment between two specified endpoints
    /// </summary>
    public static bool IsPointOnLineSegment3D(Vector3 point, Vector3 endPoint1, Vector3 endPoint2)
    {
        float x1 = point.x - endPoint1.x;
        float y1 = point.y - endPoint1.y;
        float z1 = point.z - endPoint1.z;
        float x2 = point.x - endPoint2.x;
        float y2 = point.y - endPoint2.y;
        float z2 = point.z - endPoint2.z;

        return IsEqualRatio(x1, y1, x2, y2) && IsEqualRatio(x1, z1, x2, z2) && IsEqualRatio(y1, z1, y2, z2);
    }
}
