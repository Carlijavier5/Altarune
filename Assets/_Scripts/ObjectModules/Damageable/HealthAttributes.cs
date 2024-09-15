using System.Collections.Generic;
using UnityEngine;

public enum ElementType { Physical, Fire, Ice, Poison }

[System.Serializable]
public class HealthAttributes {

    [SerializeField] protected AttributeCurves curves;

    public int health = 10;
    [Range(0, 1)] public float defense;
    [Range(0, 1)] public float fireRes;
    [Range(0, 1)] public float iceRes;
    [Range(0, 1)] public float poisonRes;
    [Range(0, 1)] public float healModifier;

    public HealthAttributes(AttributeCurves curves) {
        this.curves = curves;
    }

    public HealthAttributes Clone() => MemberwiseClone() as HealthAttributes;
    public RuntimeHealthAttributes RuntimeClone(IEnumerable<StatusEffect> effectSource = null) => new(this, curves, effectSource);
}

[System.Serializable]
public class AttributeCurves {
    public AnimationCurve defenseCurve, fireResCurve, iceResCurve,
                          poisonResCurve, healModCurve;
    public AttributeCurves Clone() => (AttributeCurves) MemberwiseClone();
}