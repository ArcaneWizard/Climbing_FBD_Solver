using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class SelectHolds : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private TextMeshProUGUI modeTxt;

    public Transform selectedHold { get; private set; }
    private HoldSettings selectedHoldSettings;
    private LayerMask holdsLM = (1 << 6);

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
        if (selectedHold)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                selectedHoldSettings.SetToArmType();
            else if (Input.GetKeyDown(KeyCode.W))
                selectedHoldSettings.SetToFeetType();
        }
    }

    // select a hold (or deselect a hold if clicking nothing)
    private void select()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, holdsLM))
            {
                if (selectedHold)
                    selectedHoldSettings?.EnableOutline(false);

                selectedHold = hit.transform.parent;
                selectedHoldSettings = selectedHold.GetComponent<HoldSettings>();
                selectedHoldSettings.EnableOutline(true);

                isDragging = true;
                initialDragOffset = camera.ScreenToWorldPoint(Input.mousePosition) - selectedHold.transform.position;
            }
            else
            {
                selectedHoldSettings?.EnableOutline(false);
                selectedHold = null;
            }
        }
    }

    public void Select(Transform hold)
    {
        if (selectedHold)
        {
            Debug.Log("bruh");
            selectedHoldSettings?.EnableOutline(false);
        }

        selectedHold = hold;
        selectedHoldSettings = selectedHold.GetComponent<HoldSettings>();
        selectedHoldSettings.EnableOutline(true);

        isDragging = true;
        initialDragOffset = camera.ScreenToWorldPoint(Input.mousePosition) - selectedHold.transform.position;
    }

    private void drag()
    {
        if (!selectedHold)
            return;

        if (isDragging && Input.GetMouseButton(0))
            selectedHold.position = camera.ScreenToWorldPoint(Input.mousePosition) - initialDragOffset;

        if (Input.GetMouseButtonUp(0))
            isDragging = false;
    }

    private void rotate()
    {
        if (!selectedHold)
            return;

        if (Input.GetMouseButton(1) || enableRotationMode)
        {
            Vector3 worldMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = new Vector2(worldMousePos.x - selectedHold.position.x, worldMousePos.y - selectedHold.position.y);
            selectedHold.up = dir;

            modeTxt.text = "Mode: Rotation";
        }
        else
            modeTxt.text = "Mode: Movement";
    }

    private void resize()
    {
        if (!selectedHold)
            return;

        Vector3 localScale = selectedHold.transform.GetChild(0).localScale;
        float newXScale = localScale.x + Input.mouseScrollDelta.y * RESIZE_SPEED;
        selectedHold.transform.GetChild(0).localScale = new Vector3(newXScale, localScale.y, localScale.z);
    }
}