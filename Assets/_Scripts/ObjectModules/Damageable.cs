using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : ObjectModule {

    [SerializeField] private HealthMeter healthMeter;
    [SerializeField] private HealthAttributes hpAttributes;

    void Awake() {
        baseObject.OnTryDamage += BaseObject_OnTryDamage;
    }

    private bool BaseObject_OnTryDamage(int amount, ElementType element) {
        healthMeter.TakeDamage(amount, element);
        return true;
    }

    #if UNITY_EDITOR
    protected override void Reset() {
        base.Reset();
        CJUtils.AssetUtils.TryRetrieveAsset(out AttributeCurveData curves);
        hpAttributes = new(curves);
    }
    #endif
}

public enum ElementType { Physical, Fire, Ice, Poison }

[System.Serializable]
public class HealthAttributes {

    [SerializeField] private AttributeCurves curves;

    public int health;
    [Range(0, 1)] public float defense;

    public HealthAttributes(AttributeCurveData curveData) {
        curves = curveData.Data;
    }

    public int ComputeDamage(int amount) {
        return Mathf.FloorToInt(amount * curves.defenseCurve.Evaluate(defense));
    }
}

[CreateAssetMenu()]
public class AttributeCurveData : ScriptableObject {
    [SerializeField] private AttributeCurves data;
    public AttributeCurves Data => data.Clone();
}

public class AttributeCurves {
    public AnimationCurve defenseCurve;
    public AttributeCurves Clone() => (AttributeCurves) MemberwiseClone();
}