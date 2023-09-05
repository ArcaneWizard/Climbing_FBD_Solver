using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldSpawner : MonoBehaviour
{
    [SerializeField] private Transform prefabHold;
    private HashSet<Transform> holds;
    private SelectHolds selectHolds;

    void Awake()
    {
        holds = new HashSet<Transform>();
        selectHolds = transform.GetComponent<SelectHolds>();
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform hold = transform.GetChild(i);
            holds.Add(hold);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            AddHold();
        if (Input.GetKeyDown(KeyCode.S))
            RemoveHold();
    }

    private void AddHold()
    {
        Transform hold = Instantiate(prefabHold, transform.position, Quaternion.identity, transform);
        holds.Add(hold);
        selectHolds.Select(hold);
    }

    private void RemoveHold()
    {
        Transform hold = selectHolds.selectedHold;
        if (!hold || !holds.Contains(hold))
            return;

        holds.Remove(hold);
        Destroy(hold.gameObject);
    }
}
