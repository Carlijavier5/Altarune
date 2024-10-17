using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CCAttributes {

    [SerializeField] protected DefaultCrowdControlSettings ccSettings;
    [Range(0, 1)] public float ccResistance;
    [Range(0, 1)] public float stunResistance;
    [Range(0, 1)] public float rootResistance;
    [Range(0, 1)] public float slowResistance;

    public float CCUpdateFrequency => ccSettings.ccUpdateFrequency;

    public CCAttributes(DefaultCrowdControlSettings ccSettings) {
        this.ccSettings = ccSettings;
    }

    public CCAttributes Clone() => MemberwiseClone() as CCAttributes;
    public RuntimeCCAttributes RuntimeClone(IEnumerable<StatusEffect> effectSource) => new(this, ccSettings, effectSource);
}