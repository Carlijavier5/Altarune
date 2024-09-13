using System.Collections.Generic;
using UnityEngine;

public enum ElementType { Physical, Fire, Ice, Poison }

[System.Serializable]
public class HealthAttributes {

    [SerializeField] protected AttributeCurves curves;

    public int health;
    [Range(0, 1)] public float defense;
    [Range(0, 1)] public float healModifier;

    public HealthAttributes(AttributeCurves curves) {
        this.curves = curves;
    }

    public HealthAttributes Clone() => MemberwiseClone() as HealthAttributes;
    public RuntimeHealthAttributes RuntimeClone() => new(this, curves);
}

public class RuntimeHealthAttributes : HealthAttributes {

    private readonly HealthAttributes healthAttributes;
    public int Health => healthAttributes.health;

    public RuntimeHealthAttributes(HealthAttributes defaultAttributes,
                                   AttributeCurves curves) : base(curves) {
        healthAttributes = defaultAttributes;
    }

    public int ComputeDamage(int amount) {
        int absAmount = Mathf.Abs(amount);
        return Mathf.FloorToInt(absAmount * curves.defenseCurve.Evaluate(defense));
    }

    public int ComputeHeal(int amount) {
        int absAmount = Mathf.Abs(amount);
        return Mathf.FloorToInt(absAmount * curves.healModCurve.Evaluate(healModifier));
    }

    public int DoDamage(int amount) {
        int processedAmount = ComputeDamage(amount);
        health = Mathf.Max(0, health - processedAmount);
        return health > 0 ? processedAmount : -1;
    }

    public int DoHeal(int amount) {
        int processedAmount = ComputeHeal(amount);
        health = Mathf.Min(healthAttributes.health, health + processedAmount);
        return processedAmount;
    }
}

[System.Serializable]
public class AttributeCurves {
    public AnimationCurve defenseCurve, healModCurve;
    public AttributeCurves Clone() => (AttributeCurves) MemberwiseClone();
}