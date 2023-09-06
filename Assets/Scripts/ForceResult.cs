using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceResult : IComparable<ForceResult>
{
    public float MaxHandForce { get; set; }
    public float MinHandForce { get; set; }
    public float Hand1, Hand2, Foot1, Foot2;

    public ForceResult(float hand1, float hand2, float foot1, float foot2)
    {
        Hand1 = hand1;
        Hand2 = hand2;
        Foot1 = foot1;
        Foot2 = foot2;

        MaxHandForce =  Math.Max(hand1, hand2);
        MinHandForce= Math.Min(hand1, hand2);
    }

    public ForceResult(Vector4 forces)
    {
        Hand1 = forces.x;
        Hand2 = forces.y;
        Foot1 = forces.z;
        Foot2 = forces.w;

        MaxHandForce = Math.Max(Math.Abs(Hand1), Math.Abs(Hand2));
        MinHandForce = Math.Min(Math.Abs(Hand1), Math.Abs(Hand2));
    }

    public int CompareTo(ForceResult other)
    {
        if (MaxHandForce != other.MaxHandForce)
            return MaxHandForce.CompareTo(other.MaxHandForce);
        else
            return MinHandForce.CompareTo(other.MinHandForce);
    }
}
