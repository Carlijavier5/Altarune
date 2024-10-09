﻿#if UNITY_EDITOR
using UnityEngine;

public class DefaultHealthAttributeCurves : ScriptableObject {
    public AnimationCurve defenseCurve, fireResCurve, iceResCurve,
                          shockResCurve, poisonResCurve, healModCurve;
}
#endif