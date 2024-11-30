using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PushableAttributes {
    public DefaultEaseCurves easeCurves;
    [Min(0.1f)] public float objectMass = 1;
    [Range(0, 1)] public float pushResistance;

    public PushableAttributes(DefaultEaseCurves easeCurves) {
        this.easeCurves = easeCurves;
    }

    public PushableAttributes Clone() => MemberwiseClone() as PushableAttributes;
    public RuntimePushAttributes RuntimeClone(IEnumerable<EntityEffect> effectSource = null) => new(this, easeCurves, effectSource);
}
