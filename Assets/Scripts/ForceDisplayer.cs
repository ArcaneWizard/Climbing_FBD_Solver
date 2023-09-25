using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ForceDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private List<PositionResult> results;
    private int idx;

    private PositionsSolver solver;
    private HoldSpawner holdSpawner;

    void Awake()
    {
        solver = transform.GetComponent<PositionsSolver>();
        holdSpawner = transform.GetComponent<HoldSpawner>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            results = solver.SolvePositions();
            idx = 0;
            displayNewForceResults();
        }

        int offset = 0;

        if ((Input.GetKeyDown(KeyCode.Alpha2)))
            offset -= 1;

        if ((Input.GetKeyDown(KeyCode.Alpha3)))
            offset += 1;

        if (results?.Count > 0 && offset != 0)
        {
            idx = MathX.Modulo(idx + offset, results.Count);
            displayNewForceResults();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            results = null;
            foreach (Transform hold in holdSpawner.AllHolds)
            {
                hold.GetComponent<Hold>().SetForce(1);
                hold.GetComponent<Hold>().EnableForceArrow(true);
            }
        }

        displayResultText();
    }

    private void displayResultText()
    {
        if (results != null && results.Count > 0)
            text.text = $"Position {idx + 1}\n" + results[idx].ToString();
        else
            text.text = "No positions yet";
    }

    private void displayNewForceResults()
    {
        foreach (Transform hold in holdSpawner.AllHolds)
            hold.GetComponent<Hold>().EnableForceArrow(false);

        if (results == null || results.Count <= 0)
            return;

        foreach (HoldInfo i in results[idx].holdsInfo)
        {
            i.Hold.SetForce(i.Force);
            i.Hold.EnableForceArrow(true);
        }
    }
}
