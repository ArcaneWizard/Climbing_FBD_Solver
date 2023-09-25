using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using cakeslice;

public class Body : SelectableObject
{
    [field: SerializeField] public Transform shoulder1;
    [field: SerializeField] public Transform shoulder2;
    [field: SerializeField] public Transform hip1;
    [field: SerializeField] public Transform hip2;

    public Vector2 shoulder1Pos;
    public Vector2 shoulder2Pos;
    public Vector2 hip1Pos;
    public Vector2 hip2Pos;

    public Vector2 GetCenterOfShoulders() {
        return new Vector2(
            (shoulder1.position.x + shoulder2.position.x) / 2f,
            (shoulder1.position.y + shoulder2.position.y) / 2f );
     }

    public Vector2 GetCenterOfHips()
    {
        return new Vector2(
            (hip1.position.x + hip2.position.x) / 2f,
            (hip1.position.y + hip2.position.y) / 2f);
    }

    public Vector2 GetClosestShoulder(Vector2 pos)
    {
        if (Vector2.Distance(pos, shoulder1.position) <= Vector2.Distance(pos, shoulder2.position))
            return shoulder1.position;
        else
            return shoulder2.position;
    }

    public Vector2 GetClosestHip(Vector2 pos)
    {
        if (Vector2.Distance(pos, hip1.position) <= Vector2.Distance(pos, hip2.position))
            return hip1.position;
        else
            return hip2.position;
    }


    public Vector3 GetCOG() => transform.position;
}
