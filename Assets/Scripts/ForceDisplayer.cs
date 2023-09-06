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

    void Awake()
    {
        calculator = transform.GetComponent<ForceCalculator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            results = calculator.CalculateIdealForces();
            idx = 0;
        }

        if (Input.GetKeyDown(KeyCode.X) && results?.Count > 0)
            idx = (idx + 1) % results.Count;

        displayResultText();

        if (results?.Count > 0)
            displayForceResultsOnHolds();
    }

    private void displayResultText()
    {
        if (results != null && results.Count > 0)
            text.text = $"Result {idx + 1}.\n" + results[idx].ToString();
        else
            text.text = "No results yet";
    }

    private void displayForceResultsOnHolds()
    {
        foreach (HoldInfo i in results[idx].holdsInfo)
            i.Hold.SetForce(i.Force);
    }
}
