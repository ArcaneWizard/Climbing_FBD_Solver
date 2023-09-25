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

    public SelectedObject selected { get; private set; }
    private LayerMask holdsOrBody = (1 << 6) | (1 << 7);

    private bool isDragging;
    private bool enableRotationMode;
    private Vector3 initialDragOffset;
    private const float RESIZE_SPEED = 0.6f;

    public void Select(Transform t)
    {
        if (selected != null)
            selected.Object?.EnableOutline(false);

        selected = new SelectedObject(t, t.GetComponent<SelectableObject>());
        selected.Object?.EnableOutline(true);

        isDragging = true;
        initialDragOffset = camera.ScreenToWorldPoint(Input.mousePosition) - selected.Transform.position;
    }

    void Start() => modeTxt.text = "Mode: Movement";

    void Update()
    {
        if (selected?.Transform == null)
            selected = null;

        select();
        drag();
        rotate();
        resize();

        if (Input.GetKeyDown(KeyCode.R))
            enableRotationMode = !enableRotationMode;

        // controls to change the selected hold's limb type
        if (selected != null && selected.Object.IsHold())
        {
            if (Input.GetKeyDown(KeyCode.Q))
                ((Hold)selected.Object).SetLimbType(HoldLimbType.Hand);
            else if (Input.GetKeyDown(KeyCode.W))
                ((Hold)selected.Object).SetLimbType(HoldLimbType.Foot);
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
                if (selected != null)
                    selected.Object?.EnableOutline(false);

                Transform t = hit.transform.parent;
                selected = new SelectedObject(t, t.GetComponent<SelectableObject>());
                selected.Object.EnableOutline(true);

                isDragging = true;
                initialDragOffset = camera.ScreenToWorldPoint(Input.mousePosition) - selected.Transform.position;
            }
            else
            {
                selected?.Object?.EnableOutline(false);
                selected = null;
            }
        }
    }

    private void drag()
    {
        if (selected == null)
            return;

        if (isDragging && Input.GetMouseButton(0))
            selected.Transform.position = camera.ScreenToWorldPoint(Input.mousePosition) - initialDragOffset;

        if (Input.GetMouseButtonUp(0))
            isDragging = false;
    }

    private void rotate()
    {
        if (selected == null)
            return;

        if (Input.GetMouseButton(1) || enableRotationMode)
        {
            Vector3 worldMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = new Vector2(worldMousePos.x - selected.Transform.position.x, worldMousePos.y - selected.Transform.position.y);
            selected.Transform.up = dir;

            modeTxt.text = "Mode: Rotation";
        }
        else
            modeTxt.text = "Mode: Movement";
    }

    private void resize()
    {
        if (selected == null || selected.Object.IsHold())
            return;

        Vector3 localScale = selected.Transform.GetChild(0).localScale;
        float newXScale = localScale.x + Input.mouseScrollDelta.y * RESIZE_SPEED;
        selected.Transform.GetChild(0).localScale = new Vector3(newXScale, localScale.y, localScale.z);
    }
}

public class SelectedObject
{
    public Transform Transform { get; private set; }
    public SelectableObject Object { get; private set; }

    public SelectedObject(Transform t, SelectableObject obj)
    {
        Transform = t;
        Object = obj;
    }
}