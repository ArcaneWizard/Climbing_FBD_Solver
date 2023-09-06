using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using cakeslice;

public class BodySettings : ISettings
{
    [SerializeField] private Transform shoulder1;
    [SerializeField] private Transform shoulder2;
    [SerializeField] private Transform hip1;
    [SerializeField] private Transform hip2;

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

    public Vector3 GetCOG() => transform.position;
}
