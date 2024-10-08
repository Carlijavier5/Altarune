using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimePushAttributes : PushAttributes {

    private readonly PushAttributes pushAttributes;
    private IEnumerable<StatusEffect> effectSource;

    public RuntimePushAttributes(PushAttributes defaultAttributes, DefaultEaseCurves curves,
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
            if (statusEffect.CCMods != null) {
                composite.Compose(statusEffect.CCMods);
            }
        }

        composite.ApplyModifiers(this, pushAttributes);
    }
}
