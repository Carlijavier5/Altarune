using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthAttributes {

    [SerializeField] protected HealthAttributeCurves curves;

    public int health = 10;
    [Range(0, 1)] public float defense;
    [Range(0, 1)] public float fireRes;
    [Range(0, 1)] public float iceRes;
    [Range(0, 1)] public float shockRes;
    [Range(0, 1)] public float poisonRes;
    [Range(0, 1)] public float healModifier;

    public HealthAttributes(HealthAttributeCurves curves) {
        this.curves = curves;
    }

    public HealthAttributes Clone() => MemberwiseClone() as HealthAttributes;
    public RuntimeHealthAttributes RuntimeClone(IEnumerable<StatusEffect> effectSource = null) => new(this, curves, effectSource);
}

[System.Serializable]
public class HealthAttributeCurves {
    public AnimationCurve defenseCurve, fireResCurve, iceResCurve,
                          shockResCurve, poisonResCurve, healModCurve;
    public HealthAttributeCurves Clone() => (HealthAttributeCurves) MemberwiseClone();
}