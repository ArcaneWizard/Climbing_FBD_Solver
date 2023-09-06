using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using Unity.VisualScripting;
using TMPro;

public class Hold : SelectableObject
{
    [field: SerializeField] public HoldLimbType LimbType { get; private set; }
    private float force;
    private bool enableForceArrow;
    private const float ARROW_SIZE_FOR_BODY_WEIGHT = 2f;

    private LineRenderer forceArrow;
    private TextMeshPro forceReading;
    private Renderer renderer;

    [SerializeField] private Material armMaterial;
    [SerializeField] private Material footMaterial;

    public void SetForce(float force) => this.force = force;

    public void EnableForceArrow(bool on) => enableForceArrow = on;

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

    protected override void Awake()
    {
        base.Awake();
        renderer = transform.GetChild(0).GetComponent<Renderer>();
        forceArrow = transform.GetComponent<LineRenderer>();
        forceReading = transform.GetChild(1).GetComponent<TextMeshPro>();
    }

    void Start()
    {
        enableForceArrow = true;
        force = 1f;
    }

    void Update()
    {
        forceArrow.SetPosition(1, new Vector3(0, force * ARROW_SIZE_FOR_BODY_WEIGHT * (enableForceArrow ? 1 : 0), 0));
        float width = force;
        if (width > 1f)
            width = 1f;
        else if (width < 0.15f)
            width = 0.15f;

        forceArrow.startWidth = width;
        forceArrow.endWidth = width;

        forceReading.text = string.Format("{0:F2}", force);
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
}