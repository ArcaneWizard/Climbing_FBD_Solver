using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PositionSolver : MonoBehaviour
{
    private Body body;

    public PositionSolver(Body body)
    {
        this.body = body;
    }

    // Note: hold1 is pivot hold
    private void solveTwoLimbs(Transform hold1, Transform hold2) {}

    /// <summary> Note: first footHold is pivot foot hold  </summary>
    public void SolveThreeLimbs(List<Transform> armHold, List<Transform> footHold) { }

    /// <summary> 
    /// Takes in 2 arms, 2 feet, pivot leg force, static friction coefficient (mu), and 4 friction multipliers for the 4 limbs (f).
    /// Note: the pivot leg force is applied to foot1. 
    /// </summary>
    public PositionResult SolveFourLimbs(Transform arm1, Transform arm2, Transform foot1, Transform foot2, float pivotForce, float mu, float[] f)
    {
        // pivot axis spans from pivot foot hold to center of gravity
        Vector2 pivotAxis = (body.GetCOG() - foot1.position).normalized;

        // the if statement checks if pivot axis is more or less aligned with upper body axis
        //Vector2 armForcePos = body.GetCenterOfShoulders();
        //if (Vector2.Dot(pivotAxis.normalized, body.transform.up.normalized) < Mathf.Cos(maxSafeRotationOfUpperBody))
            //armForcePos = body.GetCOG();

        Vector2 hipPos = body.GetCenterOfHips();
        float gravityTorque = Forces.CalculateTorqueAroundInfiniteAxis2D(Vector2.down, body.GetCOG(), pivotAxis, foot1.position);

        // max static friction = mu * Normal force
        // static friction <= mu * Normal force

        Matrix4x4 a = Matrix4x4.identity;
        a[0, 0] = Forces.CalculateComponent2D(arm1.up.normalized, Vector2.up) + mu * f[0] * Forces.CalculateComponent2D(arm1.right.normalized, Vector2.up);
        a[0, 1] = Forces.CalculateComponent2D(arm2.up.normalized, Vector2.up) + mu * f[1] * Forces.CalculateComponent2D(arm2.right.normalized, Vector2.up);
        a[0, 2] = Forces.CalculateComponent2D(foot1.up.normalized, Vector2.up) + mu * f[2] * Forces.CalculateComponent2D(foot1.right.normalized, Vector2.up);
        a[0, 3] = Forces.CalculateComponent2D(foot2.up.normalized, Vector2.up) + mu * f[3] * Forces.CalculateComponent2D(foot2.right.normalized, Vector2.up);

        a[1, 0] = Forces.CalculateComponent2D(arm1.up.normalized, Vector2.right) + mu * f[0] * Forces.CalculateComponent2D(arm1.right.normalized, Vector2.up);
        a[1, 1] = Forces.CalculateComponent2D(arm2.up.normalized, Vector2.right) + mu * f[1] * Forces.CalculateComponent2D(arm2.right.normalized, Vector2.up);
        a[1, 2] = Forces.CalculateComponent2D(foot1.up.normalized, Vector2.right) + mu * f[2] * Forces.CalculateComponent2D(foot1.right.normalized, Vector2.up);
        a[1, 3] = Forces.CalculateComponent2D(foot2.up.normalized, Vector2.right) + mu * f[3] * Forces.CalculateComponent2D(foot2.right.normalized, Vector2.up);

        a[2, 0] = Forces.CalculateTorqueAroundInfiniteAxis2D(arm1.up.normalized + mu * f[0] * arm1.right.normalized, body.shoulder1.position, pivotAxis, foot1.position);
        a[2, 1] = Forces.CalculateTorqueAroundInfiniteAxis2D(arm2.up.normalized + mu * f[1] * arm2.right.normalized, body.shoulder2.position, pivotAxis, foot1.position);
        a[2, 2] = Forces.CalculateTorqueAroundInfiniteAxis2D(foot1.up.normalized + mu * f[2] * foot1.right.normalized, body.hip1.position, pivotAxis, foot1.position);
        a[2, 3] = Forces.CalculateTorqueAroundInfiniteAxis2D(foot2.up.normalized + mu * f[3] * foot2.right.normalized, body.hip2.position, pivotAxis, foot1.position);

        a[3, 0] = 0;
        a[3, 1] = 0;
        a[3, 2] = 1;
        a[3, 3] = 0;

        Vector4 b = new Vector4(1, 0, -gravityTorque, pivotForce);
        var solution = a.inverse * b;
        return new PositionResult(solution, arm1, arm2, foot1, foot2);
    }
}
