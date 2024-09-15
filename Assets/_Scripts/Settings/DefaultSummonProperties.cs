#if UNITY_EDITOR
using UnityEngine;

public class DefaultSummonProperties : ScriptableObject {
    public Material fadeMaterial;
    public AnimationCurve growthCurveXZ, growthCurveY;
}
#endif