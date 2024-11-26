using UnityEngine;
using UnityEngine.VFX;

public class DefaultTeleportableSettings : ScriptableObject {
    public Material material;
    public VisualEffect teleportEffect;
    public AnimationCurve scaleCurveXZ, scaleCurveY;
    public float duration;
}