using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldSpawner : MonoBehaviour
{
    [SerializeField] private Transform prefabHold;
    public HashSet<Transform> AllHolds { get; set; }
    private Selector selector;

    void Awake()
    {
        AllHolds = new HashSet<Transform>();
        selector = transform.GetComponent<Selector>();
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform hold = transform.GetChild(i);
            AllHolds.Add(hold);
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
        AllHolds.Add(hold);
        selector.Select(hold);
    }

    private void RemoveHold()
    {
        Transform selected = selector.selected;
        if (!selected || !AllHolds.Contains(selected))
            return;

        AllHolds.Remove(selected);
        Destroy(selected.gameObject);
    }
}
