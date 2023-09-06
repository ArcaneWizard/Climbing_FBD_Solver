using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Selector : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private TextMeshProUGUI modeTxt;

    public Transform selected { get; private set; }
    private SelectableObject settings;
    private LayerMask holdsOrBody = (1 << 6) | (1 << 7);

    private bool isDragging;
    private bool enableRotationMode;
    private Vector3 initialDragOffset;
    private const float RESIZE_SPEED = 0.6f;

    void Start()
    {
        modeTxt.text = "Mode: Movement";
    }

    void Update()
    {
        select();
        drag();
        rotate();
        resize();

        if (Input.GetKeyDown(KeyCode.R))
            enableRotationMode = !enableRotationMode;

        // keys to change the selected hold's limb type
        if (selected && settings.IsHold())
        {
            if (Input.GetKeyDown(KeyCode.Q))
                ((Hold)settings).SetToArmType();
            else if (Input.GetKeyDown(KeyCode.W))
                ((Hold)settings).SetToFeetType();
        }
    }

    // select a hold (or deselect a hold if clicking nothing)
    private void select()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, holdsOrBody))
            {
                if (selected)
                    settings?.EnableOutline(false);

                selected = hit.transform.parent;
                settings = selected.GetComponent<SelectableObject>();
                settings.EnableOutline(true);

                isDragging = true;
                initialDragOffset = camera.ScreenToWorldPoint(Input.mousePosition) - selected.transform.position;
            }
            else
            {
                settings?.EnableOutline(false);
                selected = null;
            }
        }
    }

    public void Select(Transform transform)
    {
        if (selected)
            settings?.EnableOutline(false);

        selected = transform;
        settings = selected.GetComponent<SelectableObject>();
        settings.EnableOutline(true);

        isDragging = true;
        initialDragOffset = camera.ScreenToWorldPoint(Input.mousePosition) - selected.transform.position;
    }

    private void drag()
    {
        if (!selected)
            return;

        if (isDragging && Input.GetMouseButton(0))
            selected.position = camera.ScreenToWorldPoint(Input.mousePosition) - initialDragOffset;

        if (Input.GetMouseButtonUp(0))
            isDragging = false;
    }

    private void rotate()
    {
        if (!selected)
            return;

        if (Input.GetMouseButton(1) || enableRotationMode)
        {
            Vector3 worldMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = new Vector2(worldMousePos.x - selected.position.x, worldMousePos.y - selected.position.y);
            selected.up = dir;

            modeTxt.text = "Mode: Rotation";
        }
        else
            modeTxt.text = "Mode: Movement";
    }

    private void resize()
    {
        if (!selected || !settings.IsHold())
            return;

        Vector3 localScale = selected.transform.GetChild(0).localScale;
        float newXScale = localScale.x + Input.mouseScrollDelta.y * RESIZE_SPEED;
        selected.transform.GetChild(0).localScale = new Vector3(newXScale, localScale.y, localScale.z);
    }
}