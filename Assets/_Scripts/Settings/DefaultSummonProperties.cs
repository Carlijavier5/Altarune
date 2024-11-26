using UnityEngine;
using UnityEngine.VFX;

public class DefaultSummonProperties : ScriptableObject {
    public Material hologramMaterial;
    public VisualEffect hologramVFX;
    public AnimationCurve growthCurveXZ, growthCurveY;
    public float growTime;
}