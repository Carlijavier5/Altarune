using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiftlingIndicatorController : MonoBehaviour
{
    [SerializeField] private LockdownJointTransform lockdownJoint;
    [SerializeField] private TwoColoredGraphicFader[] anticipationGraphics;

    public void Toggle(bool on) {
        if (on) {
            lockdownJoint.Apply();
        }

        foreach (TwoColoredGraphicFader graphic in anticipationGraphics) {
            graphic.DoFade(on);
        }
    }
}
