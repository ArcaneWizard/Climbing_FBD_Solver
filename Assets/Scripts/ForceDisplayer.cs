using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ForceDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private List<PositionResult> results;
    private int idx;

    private ForceCalculator calculator;
    private HoldSpawner holdSpawner;

    void Awake()
    {
        calculator = transform.GetComponent<ForceCalculator>();
        holdSpawner = transform.GetComponent<HoldSpawner>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            results = calculator.CalculateIdealForces();
            idx = 0;

            if (results?.Count > 0)
                displayForceResultsOnHolds();
        }

        int offset = 0;

        if ((Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKey(KeyCode.Alpha2)))
            offset += 1;

        if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKey(KeyCode.Alpha3)))
            offset -= 1;

        if (results?.Count > 0)
            idx = (idx + 1) % results.Count;

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
            text.text = $"Position {idx + 1}.\n" + results[idx].ToString();
        else
            text.text = "No positions yet";
    }

    private void displayForceResultsOnHolds()
    {
        foreach (Transform hold in holdSpawner.AllHolds)
            hold.GetComponent<Hold>().EnableForceArrow(false);

        foreach (HoldInfo i in results[idx].holdsInfo)
        {
            i.Hold.SetForce(i.Force);
            i.Hold.EnableForceArrow(true);
        }
    }
}
