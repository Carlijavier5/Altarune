using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeHealthAttributes : HealthAttributes {

    private readonly HealthAttributes healthAttributes;
    public int Health => health;

    private IEnumerable<StatusEffect> effectSource;

    public RuntimeHealthAttributes(HealthAttributes defaultAttributes, AttributeCurves curves,
                                   IEnumerable<StatusEffect> effectSource) : base(curves) {
        healthAttributes = defaultAttributes;
        this.effectSource = effectSource;
    }

    public int ComputeDamage(int amount, ElementType element = ElementType.Physical) {
        UpdateAttributes();
        int absAmount = Mathf.Abs(amount);
        float elementMult = element switch { ElementType.Fire => curves.fireResCurve.Evaluate(fireRes),
                                             ElementType.Ice => curves.fireResCurve.Evaluate(iceRes),
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
        foreach (StatusEffect statusEffect in effectSource) {
            if (statusEffect.AttributeMods != null) {
                composite.Compose(statusEffect.AttributeMods);
            }
        }

        composite.ApplyModifiers(this, healthAttributes);
    }
}
