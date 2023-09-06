using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForceCalculator : MonoBehaviour
{
    [SerializeField] private BodySettings body;
    private HoldSpawner spawner;

    [Range(0f, 1f), SerializeField] private float pivotForce;
    [Range(0f, 1f), SerializeField] private float staticFrictionCoefficient;
    [SerializeField] private bool useStaticFriction;

    private const float MAX_SAFE_ROTATION_OF_UPPER_BODY = 30;

    void Awake() => spawner = transform.GetComponent<HoldSpawner>();

    void Start()
    {
        passTestCases();

        pivotForce = 0.5f;
        useStaticFriction = true;
        staticFrictionCoefficient = 0.3f;
    }

    // try test cases and throw error if one fails
    private void passTestCases()
    {
        float a = Forces.CalculateTorque(new Vector2(3, 2).normalized, new Vector2(0, 0), Vector2.left, new Vector2(8, 2));
        if (!isEqual(a, -2.7735f))
            Debug.LogError("Torque not precisely calculated within one thousandth of a decimal");

        float b = Forces.CalculateTorque(new Vector2(3, 2).normalized, new Vector2(0, 0), Vector2.left, new Vector2(8, 2));
        float c = Forces.CalculateTorque(new Vector2(-3, -2).normalized, new Vector2(16, 4), Vector2.left, new Vector2(8, 2));
        if (!isEqual(b, c))
            Debug.LogError("Torque with equal, opposite diagonal forces around pivot point don't match");

        b = Forces.CalculateTorque(new Vector2(5, -1).normalized, new Vector2(0, 6), Vector2.right, new Vector2(10, 3));
        c = Forces.CalculateTorque(new Vector2(-5, 1).normalized, new Vector2(20, 0), Vector2.right, new Vector2(10, 3));
        if (!isEqual(b, c))
            Debug.LogError("Torque with equal, opposite diagonal forces around pivot point don't match");
    }

    // returns whether the two specified floats are equal (negligible difference)
    private bool isEqual(float a, float b) => Mathf.Abs(a - b) < 0.001f;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var armHolds = new List<Transform>();
            var footHolds = new List<Transform>();

            foreach (Transform hold in spawner.Holds)
            {
                if (hold.GetComponent<HoldSettings>().LimbType == HoldLimbType.Arms)
                    armHolds.Add(hold);
                else
                    footHolds.Add(hold);
            }

            for (int i = -50; i <= 50; i++)
                solveFourLimbs(armHolds[0], armHolds[1], footHolds[0], footHolds[1], i/50f);
        }
    }


    // Note: hold1 is pivot hold
    private void solveTwoLimbs(Transform hold1, Transform hold2) {}

    // Note: first footHold is pivot foot hold
    private void solveThreeLimbs(List<Transform> armHold, List<Transform> footHold) { }

    // Note: foot1 is pivot foot
    private ForceResult solveFourLimbs(Transform arm1, Transform arm2, Transform foot1, Transform foot2, float frictionMultiplier)
    {
        Debug.Log(body.GetCOG());

        // pivot axis spans from pivot foot hold to center of gravity
        Vector2 pivotAxis = (body.GetCOG() - foot1.position).normalized;

        // the if statement checks if pivot axis is more or less aligned with upper body axis
        Vector2 armForcePos = body.GetCenterOfShoulders();
        if (Vector2.Dot(pivotAxis.normalized, body.transform.up.normalized) < Mathf.Cos(MAX_SAFE_ROTATION_OF_UPPER_BODY))
            armForcePos = body.GetCOG();

        Vector2 hipPos = body.GetCenterOfHips();
        float gravityTorque = Forces.CalculateTorque(Vector2.down, body.GetCOG(), pivotAxis, foot1.position);
        float mu = (useStaticFriction) ? staticFrictionCoefficient : 0f;
        mu *= frictionMultiplier;

        Matrix4x4 a = Matrix4x4.identity;
        a[0, 0] = Forces.CalculateComponent(arm1.up.normalized, Vector2.up) + mu * Forces.CalculateComponent(arm1.right.normalized, Vector2.up);
        a[0, 1] = Forces.CalculateComponent(arm2.up.normalized, Vector2.up) + mu * Forces.CalculateComponent(arm2.right.normalized, Vector2.up);
        a[0, 2] = Forces.CalculateComponent(foot1.up.normalized, Vector2.up) + mu * Forces.CalculateComponent(foot1.right.normalized, Vector2.up);
        a[0, 3] = Forces.CalculateComponent(foot2.up.normalized, Vector2.up) + mu * Forces.CalculateComponent(foot2.right.normalized, Vector2.up);

        a[1, 0] = Forces.CalculateComponent(arm1.up.normalized, Vector2.right) + mu * Forces.CalculateComponent(arm1.right.normalized, Vector2.up);
        a[1, 1] = Forces.CalculateComponent(arm2.up.normalized, Vector2.right) + mu * Forces.CalculateComponent(arm2.right.normalized, Vector2.up);
        a[1, 2] = Forces.CalculateComponent(foot1.up.normalized, Vector2.right) + mu * Forces.CalculateComponent(foot1.right.normalized, Vector2.up);
        a[1, 3] = Forces.CalculateComponent(foot2.up.normalized, Vector2.right) + mu * Forces.CalculateComponent(foot2.right.normalized, Vector2.up);

        a[2, 0] = Forces.CalculateTorque(arm1.up.normalized + mu * arm1.right.normalized, armForcePos, pivotAxis, foot1.position);
        a[2, 1] = Forces.CalculateTorque(arm2.up.normalized + mu * arm2.right.normalized, armForcePos, pivotAxis, foot1.position);
        a[2, 2] = Forces.CalculateTorque(foot1.up.normalized + mu * foot1.right.normalized, hipPos, pivotAxis, foot1.position);
        a[2, 3] = Forces.CalculateTorque(foot2.up.normalized + mu * foot2.right.normalized, hipPos, pivotAxis, foot1.position);

        a[3, 0] = 0;
        a[3, 1] = 0;
        a[3, 2] = 1;
        a[3, 3] = 0;

        Vector4 b = new Vector4(1, 0, -gravityTorque, pivotForce);
        var solution = a.inverse * b;
        return new ForceResult(solution);

        /* 4 equations to solve:
        
         equation 1: net up force generated from holds = body weight
         setup: get all 4 force vertical component multipliers: (a,b,c,d). 
         aw + bx + cy + dz = 1

         equation 2: net horizontal force generated from holds = 0
         setup: get all 4 force horizontal component multipliers: (a,b,c,d). 
         aw + bx + cy + dz = 0 

         equation 3: net torque around pivot leg = 0
         step: get all 4 torque component multipliers: (a,b,c,d)
         get torque of gravity: (g_torque)
         aw + bx + cy + dz = -g_torque

          equation 4: specify the exact force of the pivot leg (to play around with during simulation)
          y = foot1Force  */
    }


    /*
    // Note: foot1 is pivot foot
    private void solveFourLimbs2(Transform arm1, Transform arm2, Transform foot1, Transform foot2, float foot1Force)
    {
        Debug.Log(body.GetCOG());

        // pivot axis spans from pivot foot hold to center of gravity
        Vector2 pivotAxis = (body.GetCOG() - foot1.position).normalized;

        // the if statement checks if pivot axis is more or less aligned with upper body axis
        Vector2 armForcePos = body.GetCenterOfShoulders();
        if (Vector2.Dot(pivotAxis.normalized, body.transform.up.normalized) < Mathf.Cos(MAX_SAFE_ROTATION_OF_UPPER_BODY))
            armForcePos = body.GetCOG();

        Vector2 hipPos = body.GetCenterOfHips();
        float gravityTorque = calculateTorque(Vector2.down, body.GetCOG(), pivotAxis, foot1.position);

        Matrix4x4 a = Matrix4x4.identity;
        a[0, 0] = calculateComponent(arm1.up.normalized, Vector2.up);
        a[0, 1] = calculateComponent(arm2.up.normalized, Vector2.up);
        a[0, 2] = calculateComponent(foot1.up.normalized, Vector2.up);
        a[0, 3] = calculateComponent(foot2.up.normalized, Vector2.up);

        a[1, 0] = calculateComponent(arm1.up.normalized, Vector2.right);
        a[1, 1] = calculateComponent(arm2.up.normalized, Vector2.right);
        a[1, 2] = calculateComponent(foot1.up.normalized, Vector2.right);
        a[1, 3] = calculateComponent(foot2.up.normalized, Vector2.right);

        a[2, 0] = calculateTorque(arm1.up.normalized, armForcePos, pivotAxis, foot1.position);
        a[2, 1] = calculateTorque(arm2.up.normalized, armForcePos, pivotAxis, foot1.position);
        a[2, 2] = calculateTorque(foot1.up.normalized, hipPos, pivotAxis, foot1.position);
        a[2, 3] = calculateTorque(foot2.up.normalized, hipPos, pivotAxis, foot1.position);

        a[3, 0] = 0;
        a[3, 1] = 0;
        a[3, 2] = 1;
        a[3, 3] = 0;

        Vector4 b = new Vector4(1, 0, -gravityTorque, foot1Force);
        var solution = a.inverse * b;

        Debug.Log(solution.ToString());

        /* 4 equations to solve:
        
         equation 1: net up force generated from holds = body weight
         setup: get all 4 force vertical component multipliers: (a,b,c,d). 
         aw + bx + cy + dz = 1

         equation 2: net horizontal force generated from holds = 0
         setup: get all 4 force horizontal component multipliers: (a,b,c,d). 
         aw + bx + cy + dz = 0 

         equation 3: net torque around pivot leg = 0
         step: get all 4 torque component multipliers: (a,b,c,d)
         get torque of gravity: (g_torque)
         aw + bx + cy + dz = -g_torque

          equation 4: specify the exact force of the pivot leg (to play around with during simulation)
          y = foot1Force  */

}
