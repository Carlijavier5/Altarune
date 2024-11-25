using UnityEngine;

public class DefaultCrowdControlSettings : ScriptableObject {
    public AnimationCurve ccDRCurve, ccResCurve, stunResCurve, rootResCurve, slowResCurve;
    public float staggerTime, ccUpdateFrequency;
}