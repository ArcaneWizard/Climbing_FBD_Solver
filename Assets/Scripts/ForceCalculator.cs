using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForceCalculator : MonoBehaviour
{
    private HoldSpawner spawner;
    private Transform centerOfGravity;
    private Transform upperBody;

    private const float MAX_SAFE_ROTATION_OF_UPPER_BODY = 30;

    void Awake()
    {
        spawner = transform.GetComponent<HoldSpawner>();
    }

    private void solveForceEquations()
    {
        var armHolds = new List<Transform>();
        var footHolds = new List<Transform>();

        foreach (Transform hold in spawner.Holds)
        {
            if (hold.GetComponent<Settings>().LimbType == HoldLimbType.Arms)
                armHolds.Add(hold);
            else
                footHolds.Add(hold);
        }
    }

    // Note: hold1 is pivot hold
    private void solveTwoLimbs(Transform hold1, Transform hold2)
    {

    }

    // Note: first footHold is pivot foot hold
    private void solveThreeLimbs(List<Transform> armHold, List<Transform> footHold)
    {
      
    }

    // Note: first footHold is pivot foot hold
    private void solveFourLimbs(List<Transform> armHold, List<Transform> footHold )
    {
        // axis dir = from pivot foot hold to center of gravity
        Vector2 axisDir = centerOfGravity.position - footHold[0].position;

        bool useForce
        if (Vector2.Dot(axisDir.normalized, upperBody.right.normalized) > Mathf.Cos(MAX_SAFE_ROTATION_OF_UPPER_BODY))


        //  if gravity axis is more or less aligned with upper body axis, arm holds force pos = upper body, else arm holds force pos = 
        // center of gravity

        //Debug.Log(CalculateTorque(new Vector2(3, 2).normalized, new Vector2(0,0), Vector2.right, new Vector2(8, 2)));
        //Debug.Log(CalculateTorque(new Vector2(3, -2).normalized, new Vector2(0, 4), Vector2.right, new Vector2(8, 2)));

        // step 1: get all 4 force vertical component multipliers: (a,b,c,d). 
        // equation of net upwards force = 0
        // aw + bx + cy + dz = 1

        // step 2: get all 4 force horizontal component multipliers: (a,b,c,d). 
        // equation of net right force = 0
        // aw + bx + cy + dz = 0 

        // step 3: get all 4 torque component multipliers: (a,b,c,d)
        // get torque of gravity: (g_torque)
        // equation of net torque around 1st leg = 0
        // aw + bx + cy + dz = -g_torque

        // step 4: get all 4 torque component multipliers: (a,b,c,d)
        // get torque of gravity: (g_torque)
        // equation of net torque around 2nd leg = 0
        // aw + bx + cy + dz = -g_torque

        // solve for w,x,y,z

        Matrix4x4 a = Matrix4x4.identity;
        a[0,0] = CalculateTorque(armHold[0].up.normalized, armHold[0].position, 
    }


    // returns component of vector a on vector b
    public static float Component(Vector2 a, Vector2 b)
    {
        return Vector2.Dot(a, b) / b.magnitude;
    }

    public static float CalculateTorque (Vector2 forceDir, Vector2 forcePos, Vector2 axisDir, Vector2 pivotPos)
    {
        float perpendicularForce = Component(forceDir, axisDir.Perpendicular1());

        Matrix4x4 a = Matrix4x4.identity;
        a[0, 0] = -forceDir.y;
        a[0, 1] = forceDir.x;
        a[1, 0] = -axisDir.y;
        a[1, 1] = axisDir.x;
        Vector4 b = new Vector4(-forcePos.x * forceDir.y + forcePos.y * forceDir.x, -pivotPos.x * axisDir.y + pivotPos.y * axisDir.x);
        var solution = a.inverse* b;

        Vector2 intersection = new Vector2(solution.x, solution.y);
        return perpendicularForce * Vector2.Distance(intersection, pivotPos);
    }
}
