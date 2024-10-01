using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PushAttributes {
    public DefaultEaseCurves easeCurves;
    [Min(0.1f)] public float objectMass = 1;
    [Range(0, 1)] public float pushResistance;

    public PushAttributes(DefaultEaseCurves easeCurves) {
        this.easeCurves = easeCurves;
    }

    public PushAttributes Clone() => MemberwiseClone() as PushAttributes;
    public RuntimePushAttributes RuntimeClone(IEnumerable<StatusEffect> effectSource = null) => new(this, easeCurves, effectSource);
}
