using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PositionsSolver : MonoBehaviour
{
    [SerializeField] private Body body;
    private HoldSpawner spawner;
    private PositionSolver positionSolver;
    private List<PositionResult> results;

    [Range(0f, 1f), SerializeField] private float staticFrictionCoefficient;
    [Range(0f, 3f), SerializeField] private float maxHandHoldForceAllowed;
    [Range(0f, 3f), SerializeField] private float maxFootHoldForceAllowed;
    [Range(0f, 1f), SerializeField] private float pivotForce;
    private const float MAX_SAFE_ROTATION_OF_UPPER_BODY = 30;

    [SerializeField] private bool doPivotForceSearch;
    [SerializeField] private bool noHeelOrToeHook;

    void Awake()
    {
        spawner = transform.GetComponent<HoldSpawner>();
        positionSolver = new PositionSolver(body);
        results = new List<PositionResult>();
    }

    void Start()
    {
        staticFrictionCoefficient = 0.3f;
        doPivotForceSearch = true;

        pivotForce = 0.5f;
        maxHandHoldForceAllowed = 0.8f;
        maxFootHoldForceAllowed = 1.7f;
    }

    public List<PositionResult> SolvePositions()
    {
        var armHolds = new List<Transform>();
        var footHolds = new List<Transform>();

        // get all holds, and sort by arm vs foot hold
        foreach (Transform hold in spawner.AllHolds)
        {
            if (hold.GetComponent<Hold>().LimbType == HoldLimbType.Hand)
                armHolds.Add(hold);
            else
                footHolds.Add(hold);
        }

        var newResults = new List<PositionResult>();

        // calculate positions with varying pivot leg forces
        if (doPivotForceSearch)
        {
            for (float i = 0f; i <= maxFootHoldForceAllowed; i += 0.05f)
                calculatePositionsWithVaryingFriction(newResults, armHolds, footHolds, i);
        }
        else
            calculatePositionsWithVaryingFriction(newResults, armHolds, footHolds, pivotForce);

        // exit if no positions were found
        if (newResults.Count == 0)
            return null;

        // sort positions by best position
        newResults.Sort();

        // get rid of duplicate positions
        results.Clear();
        results.Add(newResults[0]);
        for (int i = 1; i < newResults.Count; i++)
        {
            if (!MathX.Equal(newResults[i].MaxHandForceMagnitude, results[results.Count - 1].MaxHandForceMagnitude))
                results.Add(newResults[i]);
            else if (!MathX.Equal(newResults[i].MinHandForceMagnitude, results[results.Count - 1].MinHandForceMagnitude))
                results.Add(newResults[i]);
        }

        return results;
    }

    private void calculatePositionsWithVaryingFriction(List<PositionResult> newResults, List<Transform> armHolds, List<Transform> footHolds, float pivotForce)
    {
        int bound = 3;
        float b = bound;
        for (int i = -bound; i <= bound; i++)
        {
            for (int j = -bound; j <= bound; j++)
            {
                for (int k = -bound; k <= bound; k++)
                {
                    for (int l = -bound; l <= bound; l++)
                    {
                        // refactor when scaling to use less holds
                        PositionResult result = positionSolver.SolveFourLimbs(armHolds[0], armHolds[1], footHolds[0], footHolds[1],
                            pivotForce, staticFrictionCoefficient, new float[4] { i / bound, j / bound, k / bound, l / bound }
                        ); ;

                        if (doesPositionSatisfyConstraints(result))
                            newResults.Add(result);
                    }
                }
            }
        }

        for (int i = -50; i <= 50; i++)
        {
            // refactor when scaling to use less holds
            PositionResult result = positionSolver.SolveFourLimbs(armHolds[0], armHolds[1], footHolds[0], footHolds[1],
                pivotForce, staticFrictionCoefficient, new float[4] { i / 50f, i / 50f, i / 50f, i / 50f }
             );

            if (doesPositionSatisfyConstraints(result))
                newResults.Add(result);
        }
    }

    private bool doesPositionSatisfyConstraints(PositionResult result)
    {
        if (result.MaxHandForceMagnitude > maxHandHoldForceAllowed || result.MaxLegForceMagnitude > maxFootHoldForceAllowed)
            return false;

        else if (noHeelOrToeHook && (result.holdsInfo[2].Force < 0 || result.holdsInfo[3].Force < 0))
            return false;

        return true;
    }
}
