using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionResult : IComparable<PositionResult>
{
    public List<HoldInfo> holdsInfo { get; set; }
    public float MaxHandForceMagnitude { get; set; }
    public float MinHandForceMagnitude { get; set; }

    // position result for 2 hand holds + 2 feet holds
    public PositionResult(Vector4 forces, Transform arm1, Transform arm2, Transform foot1, Transform foot2)
    {
        holdsInfo = new List<HoldInfo>
        {
            new HoldInfo(arm1, forces.x),
            new HoldInfo(arm2, forces.y),
            new HoldInfo(foot1, forces.z),
            new HoldInfo(foot2, forces.w)
        };

        MaxHandForceMagnitude = Math.Max(Math.Abs(forces.x), Math.Abs(forces.y));
        MinHandForceMagnitude = Math.Min(Math.Abs(forces.x), Math.Abs(forces.y));
    }

    // position result for any number of holds + hold types
    public PositionResult(List<HoldInfo> info, float maxHandForceMag, float minHandForceMag)
    {
        if (maxHandForceMag < 0 || minHandForceMag < 0)
            Debug.LogError("a force's magnitude cannot be negative");

        holdsInfo = info;
        MaxHandForceMagnitude = maxHandForceMag;
        MinHandForceMagnitude = minHandForceMag;
    }

    public override String ToString() => $"Max Hand Force: {MaxHandForceMagnitude}";

    // best position has the smallest max hand force magnitude
    public int CompareTo(PositionResult other)
    {
        if (MaxHandForceMagnitude != other.MaxHandForceMagnitude)
            return MaxHandForceMagnitude.CompareTo(other.MaxHandForceMagnitude);
        else
            return MinHandForceMagnitude.CompareTo(other.MinHandForceMagnitude);
    }
}

public class HoldInfo
{
    public Hold Hold { get; set; }
    public float Force { get; set; }

    public HoldInfo(Transform hold, float force)
    {
        Hold = hold.GetComponent<Hold>();
        Force = force;
    }
}
