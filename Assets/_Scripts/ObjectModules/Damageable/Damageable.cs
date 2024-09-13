using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : ObjectModule {

    [SerializeField] private HealthAttributes defaultHPAttributes;
    [SerializeField] private IFrameProperties iFrameProperties;
    [SerializeField] private HealthMeter healthMeter;

    private RuntimeHealthAttributes runtimeHP;
    private bool iFrameOn;

    void Awake() {
        baseObject.UpdateRendererRefs();
        baseObject.OnTryDamage += BaseObject_OnTryDamage;
        runtimeHP = defaultHPAttributes.RuntimeClone();
    }

    private bool BaseObject_OnTryDamage(int amount, ElementType element) {
        if (!iFrameOn) {
            amount = runtimeHP.DoDamage(amount);
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
            defaultHPAttributes = new(curves.DefaultCurves);
        }
        if (CJUtils.AssetUtils.TryRetrieveAsset(out DefaultIFrameProperties properties)) {
            iFrameProperties = properties.DefaultProperties;
        }
    }
    #endif
}