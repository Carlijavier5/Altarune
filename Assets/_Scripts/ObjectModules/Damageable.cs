using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : ObjectModule {

    [SerializeField] private HealthAttributes hpAttributes;
    [SerializeField] private IFrameProperties iFrameProperties;
    [SerializeField] private HealthMeter healthMeter;

    private bool iFrameOn;

    void Awake() {
        baseObject.UpdateRendererRefs();
        baseObject.OnTryDamage += BaseObject_OnTryDamage;
    }

    private bool BaseObject_OnTryDamage(int amount, ElementType element) {
        if (!iFrameOn) {
            amount = hpAttributes.ComputeDamage(amount);
            if (healthMeter) healthMeter.TakeDamage(amount);
            StartCoroutine(ISimulateIFrame());
        } return !iFrameOn;
    }

    private IEnumerator ISimulateIFrame() {
        iFrameOn = true;
        baseObject.SetMaterial(iFrameProperties.flashMaterial);
        yield return new WaitForSeconds(iFrameProperties.duration);
        baseObject.ResetMaterials();
        iFrameOn = false;
    }

    #if UNITY_EDITOR
    protected override void Reset() {
        base.Reset();
        if (CJUtils.AssetUtils.TryRetrieveAsset(out DefaultAttributeCurves curves)) {
            hpAttributes = new(curves.DefaultCurves);
        }
        if (CJUtils.AssetUtils.TryRetrieveAsset(out DefaultIFrameProperties properties)) {
            iFrameProperties = properties.DefaultProperties;
        }

    }
    #endif
}