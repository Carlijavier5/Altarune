using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class Summon {

    public HashSet<SummonEffect> StatusEffects { get; private set; } = new();
    private readonly Stack<SummonEffect> terminateStack = new();

    private void UpdateEffects() {
        foreach (SummonEffect statusEffect in StatusEffects) {
            if (statusEffect.Update(this)) {
                statusEffect.Terminate(this);
                terminateStack.Push(statusEffect);
            }
            if (Perished) break;
        }

        while (terminateStack.TryPop(out SummonEffect deprecatedEffect)) {
            StatusEffects.Remove(deprecatedEffect);
        }
    }

    public void ApplyEffects(SummonEffect[] incomingEffects) {
        foreach (SummonEffect statusEffect in incomingEffects) {
            bool isNew = StatusEffects.Add(statusEffect);
            statusEffect.Apply(this, isNew);
            ///OnEffectApplied?.Invoke(statusEffect);
            statusEffect.Start(this);
        }
    }

    public void Toggle(bool on) => active = on;
}

[System.Serializable]
public abstract class SummonEffect : StatusEffect<Summon> {

    public SummonAttributeModifiers AttributeModifiers;
    public SummonCCEffects CCEffects;
}

[System.Serializable]
public class SummonAttributeModifiers {
    public AttributeModifier potency = new(), efficiency = new(), reach = new(); 

    public void ApplyModifiers(RuntimeSummonAttributes runtimeAttributes,
                               SummonAttributes defaultAttributes) {
        runtimeAttributes.potency = Mathf.Clamp01(defaultAttributes.potency * potency);
        runtimeAttributes.efficiency = Mathf.Clamp01(defaultAttributes.efficiency * efficiency);
        runtimeAttributes.reach = Mathf.Clamp01(defaultAttributes.reach * reach);
    }

    public void Compose(SummonAttributeModifiers modifiers) {
        potency.Compose(modifiers.potency);
        efficiency.Compose(modifiers.efficiency);
        reach.Compose(modifiers.reach);
    }
}

public class SummonAttributes {
    [SerializeField] protected DefaultSummonAttributeCurves curves;

    [Range(0, 100)] public float potency;
    [Range(0, 100)] public float efficiency;
    [Range(0, 100)] public float reach;

    public SummonAttributes(DefaultSummonAttributeCurves curves) {
        this.curves = curves;
    }

    public SummonAttributes Clone() => MemberwiseClone() as SummonAttributes;
    public RuntimeSummonAttributes RuntimeClone(IEnumerable<SummonEffect> effectSource = null) => new(this, curves, effectSource);
}

public class RuntimeSummonAttributes : SummonAttributes {

    private readonly SummonAttributes summonAttributes;

    private IEnumerable<SummonEffect> effectSource;

    public RuntimeSummonAttributes(SummonAttributes defaultAttributes, DefaultSummonAttributeCurves curves,
                                   IEnumerable<SummonEffect> effectSource) : base(curves) {
        summonAttributes = defaultAttributes;
        this.effectSource = effectSource;
    }
    /*
    public int ComputeAttributes(int amount, ElementType element = ElementType.Physical) {
        UpdateAttributes();
        int absAmount = Mathf.Abs(amount);
        float elementMult = element switch {
            ElementType.Fire => curves.fireResCurve.Evaluate(fireRes),
            ElementType.Ice => curves.fireResCurve.Evaluate(iceRes),
            ElementType.Shock => curves.shockResCurve.Evaluate(shockRes),
            ElementType.Poison => curves.poisonResCurve.Evaluate(poisonRes),
            _ => 1
        };
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
    */
    private void UpdateAttributes() {
        if (effectSource == null) return;

        SummonAttributeModifiers composite = new();
        foreach (SummonEffect statusEffect in effectSource) {
            if (statusEffect.AttributeModifiers != null) {
                composite.Compose(statusEffect.AttributeModifiers);
            }
        }

        composite.ApplyModifiers(this, summonAttributes);
    }
}

[CreateAssetMenu()]
public class DefaultSummonAttributeCurves : ScriptableObject {
    public AnimationCurve potencyCurve, efficiencyCurve, reachCurve;
}

public class SummonCCEffects {
    public CrowdControlNode disable;
    public SlowCCNode slow;

    public void Update(float deltaTime) {
        disable?.Update(deltaTime);
        slow?.Update(deltaTime);
    }
}