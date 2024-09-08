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
        amount = hpAttributes.ComputeDamage(amount);
        healthMeter.TakeDamage(amount);
        return true;
    }

    #if UNITY_EDITOR
    protected override void Reset() {
        base.Reset();
        CJUtils.AssetUtils.TryRetrieveAsset(out DefaultAttributeCurves curves);
        hpAttributes = new(curves.Data);
    }
    #endif
}