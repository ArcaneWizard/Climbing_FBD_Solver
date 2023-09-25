using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionResult : IComparable<PositionResult>
{
    public List<HoldInfo> holdsInfo { get; set; }
    public float MaxHandForceMagnitude { get; set; }
    public float MinHandForceMagnitude { get; set; }
    public float MaxLegForceMagnitude { get; set; }

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
        MaxLegForceMagnitude = Math.Max(Math.Abs(forces.z), Math.Abs(forces.w));
    }

    // position result for any number of holds + hold types
    public PositionResult(List<HoldInfo> info)
    {
        holdsInfo = info;

        MaxHandForceMagnitude = 0;
        MinHandForceMagnitude = Int32.MaxValue;
        MaxLegForceMagnitude = 0;

        foreach (HoldInfo i in info)
        {
            if (i.Hold.LimbType == HoldLimbType.Hand)
            {
                MaxHandForceMagnitude = Math.Max(MaxHandForceMagnitude, Mathf.Abs(i.Force));
                MinHandForceMagnitude = Math.Min(MinHandForceMagnitude, Mathf.Abs(i.Force));
            }
            else
                MaxLegForceMagnitude = Math.Max(MaxLegForceMagnitude, Mathf.Abs(i.Force));
        }

        if (MinHandForceMagnitude == Int32.MaxValue)
            MinHandForceMagnitude = 0f;
    }

    public override String ToString() => $"Max Hand Force: {MaxHandForceMagnitude}";

    // determine best position
    public int CompareTo(PositionResult other)
    {
        float score = MaxHandForceMagnitude * 7f + 4f * (MaxHandForceMagnitude + MinHandForceMagnitude);
        float otherScore = other.MaxHandForceMagnitude * 7f + 4f * (other.MaxHandForceMagnitude + other.MinHandForceMagnitude);

        return score.CompareTo(otherScore);

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
