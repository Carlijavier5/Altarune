using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimePushAttributes : PushableAttributes {

    private readonly PushableAttributes pushAttributes;
    private IEnumerable<StatusEffect> effectSource;

    public RuntimePushAttributes(PushableAttributes defaultAttributes, DefaultEaseCurves curves,
                               IEnumerable<StatusEffect> effectSource) : base(curves) {
        pushAttributes = defaultAttributes;
        this.effectSource = effectSource;
    }

    public Vector3 ComputePush(Vector3 direction) {
        UpdateAttributes();
        return (direction / objectMass) * (1 - pushResistance);
    }

    private void UpdateAttributes() {
        if (effectSource == null) return;

        CCAttributeModifiers composite = new();
        foreach (StatusEffect statusEffect in effectSource) {
            if (statusEffect.CCModifiers != null) {
                composite.Compose(statusEffect.CCModifiers);
            }
        }

        composite.ApplyModifiers(this, pushAttributes);
    }
}
