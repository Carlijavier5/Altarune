using System.Collections.Generic;
using UnityEngine;

public class ZigTrail : TwoColoredGraphicFader {

    public void Generate(Vector3 position, Quaternion rotation, float zScale) {
        transform.SetPositionAndRotation(position, rotation);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, zScale);
        DoFade(true);
    }
}