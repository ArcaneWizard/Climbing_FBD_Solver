using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Forces
{
    // returns component of vector a on vector b
    public static float CalculateComponent(Vector2 a, Vector2 b) => Vector2.Dot(a, b) / b.magnitude;

    public static float CalculateTorque(Vector2 forceDir, Vector2 forcePos, Vector2 axisDir, Vector2 pivotPos)
    {
        Matrix4x4 a = Matrix4x4.identity;
        a[0, 0] = -forceDir.y;
        a[0, 1] = forceDir.x;
        a[1, 0] = -axisDir.y;
        a[1, 1] = axisDir.x;
        Vector4 b = new Vector4(-forcePos.x * forceDir.y + forcePos.y * forceDir.x, -pivotPos.x * axisDir.y + pivotPos.y * axisDir.x);
        var solution = a.inverse * b;

        Vector2 intersection = new Vector2(solution.x, solution.y);
        float perpendicularForce = CalculateComponent(forceDir, axisDir.Perpendicular1());

        return perpendicularForce * Vector2.Distance(intersection, pivotPos) * (intersection.x > pivotPos.x ? 1 : -1);
    }
}
