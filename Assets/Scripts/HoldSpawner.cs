using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldSpawner : MonoBehaviour
{
    [SerializeField] private Transform prefabHold;
    public HashSet<Transform> Holds { get; set; }
    private Selector selectHolds;

    void Awake()
    {
        Holds = new HashSet<Transform>();
        selectHolds = transform.GetComponent<Selector>();
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform hold = transform.GetChild(i);
            Holds.Add(hold);
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
        Holds.Add(hold);
        selectHolds.Select(hold);
    }

    private void RemoveHold()
    {
        Transform hold = selectHolds.selected;
        if (!hold || !Holds.Contains(hold))
            return;

        Holds.Remove(hold);
        Destroy(hold.gameObject);
    }
}
