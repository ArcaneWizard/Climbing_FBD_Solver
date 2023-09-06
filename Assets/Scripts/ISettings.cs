using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using cakeslice;

public class ISettings : MonoBehaviour
{
    private Outline outline;

    protected virtual void Awake()
    {
        outline = transform.GetChild(0).AddComponent<Outline>();
        outline.color = 0;
    }

    public int EnableOutline(bool on) => outline.color = (on ? 1 : 0);
    public bool IsHold() => this.GetType().Equals(typeof(HoldSettings));
    public bool Is(Type other) => this.GetType().Equals(other);
}
