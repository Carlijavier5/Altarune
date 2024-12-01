using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthAttributes {

    [SerializeField] protected DefaultHealthAttributeCurves curves;

    public int health = 10;
    [Range(0, 1)] public float defense;
    [Range(0, 1)] public float fireRes;
    [Range(0, 1)] public float iceRes;
    [Range(0, 1)] public float shockRes;
    [Range(0, 1)] public float poisonRes;
    [Range(0, 1)] public float healModifier;

    public HealthAttributes(DefaultHealthAttributeCurves curves) {
        this.curves = curves;
    }

    public HealthAttributes Clone() => MemberwiseClone() as HealthAttributes;
    public RuntimeHealthAttributes RuntimeClone(IEnumerable<EntityEffect> effectSource = null) => new(this, curves, effectSource);
}