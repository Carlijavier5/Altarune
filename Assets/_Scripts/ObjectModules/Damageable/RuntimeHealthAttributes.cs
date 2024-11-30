using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeHealthAttributes : HealthAttributes {

    private readonly HealthAttributes healthAttributes;

    public int MaxHealth { get; private set; }
    public int Health => health;

    private IEnumerable<EntityEffect> effectSource;

    public RuntimeHealthAttributes(HealthAttributes defaultAttributes, DefaultHealthAttributeCurves curves,
                                   IEnumerable<EntityEffect> effectSource) : base(curves) {
        healthAttributes = defaultAttributes;
        MaxHealth = defaultAttributes.health;
        health = defaultAttributes.health;
        this.effectSource = effectSource;
    }

    public int ComputeDamage(int amount, ElementType element = ElementType.Physical) {
        UpdateAttributes();
        int absAmount = Mathf.Abs(amount);
        float elementMult = element switch { ElementType.Fire => curves.fireResCurve.Evaluate(fireRes),
                                             ElementType.Ice => curves.fireResCurve.Evaluate(iceRes),
                                             ElementType.Shock => curves.shockResCurve.Evaluate(shockRes),
                                             ElementType.Poison => curves.poisonResCurve.Evaluate(poisonRes),
                                             _ => 1 };
        return Mathf.CeilToInt(absAmount * elementMult * curves.defenseCurve.Evaluate(defense));
    }

    public int ComputeHeal(int amount) {
        UpdateAttributes();
        int absAmount = Mathf.Abs(amount);
        return Mathf.CeilToInt(absAmount * curves.healModCurve.Evaluate(healModifier));
    }

    public int DoDamage(int amount, ElementType element = ElementType.Physical) {
        int processedAmount = ComputeDamage(amount, element);
        health = Mathf.Max(0, health - processedAmount);
        return processedAmount;
    }

    public int DoHeal(int amount) {
        int processedAmount = ComputeHeal(amount);
        health = Mathf.Min(healthAttributes.health, health + processedAmount);
        return processedAmount;
    }

    private void UpdateAttributes() {
        if (effectSource == null) return;

        HealthAttributeModifiers composite = new();
        foreach (EntityEffect statusEffect in effectSource) {
            if (statusEffect.HealthModifiers != null) {
                composite.Compose(statusEffect.HealthModifiers);
            }
        }

        composite.ApplyModifiers(this, healthAttributes);
    }
}
