using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using Unity.VisualScripting;
using TMPro;

public class HoldSettings : ISettings
{
    [field: SerializeField] public HoldLimbType LimbType { get; private set; }
    [Range(0f, 2f)] public float force;
    private const float ARROW_SIZE_FOR_BODY_WEIGHT = 2f;

    private LineRenderer forceArrow;
    private TextMeshPro forceReading;
    private Renderer renderer;

    [SerializeField] private Material armMaterial;
    [SerializeField] private Material footMaterial;

    protected override void Awake()
    {
        base.Awake();
        renderer = transform.GetChild(0).GetComponent<Renderer>();
        forceArrow = transform.GetComponent<LineRenderer>();
        forceReading = transform.GetChild(1).GetComponent<TextMeshPro>();
    }

    void Start()
    {
        force = 1f;
    }

    void Update()
    {
        forceArrow.SetPosition(1, new Vector3(0, force * ARROW_SIZE_FOR_BODY_WEIGHT, 0));
        forceArrow.startWidth = Mathf.Min(force, 1f);
        forceArrow.endWidth = Mathf.Min(force, 1f);

        forceReading.text = string.Format("{0:F1}", force);
        forceReading.transform.right = Vector3.right;
    }

    void OnValidate()
    {
        renderer = transform.GetChild(0).GetComponent<Renderer>();

        if (LimbType == HoldLimbType.Arms)
            SetToArmType();
        else
            SetToFeetType();
    }


    public void SetToArmType()
    {
        LimbType = HoldLimbType.Arms;
        renderer.material = armMaterial;
    }

    public void SetToFeetType()
    {
        LimbType = HoldLimbType.Feet;
        renderer.material = footMaterial;
    }
}