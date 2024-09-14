using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : ObjectModule {

    public event System.Action<int> OnDamageTaken;

    [SerializeField] private HealthAttributes defaultHPAttributes;
    [SerializeField] private IFrameProperties iFrameProperties;

    private RuntimeHealthAttributes runtimeHP;
    private bool iFrameOn;

    void Awake() {
        baseObject.UpdateRendererRefs();
        baseObject.OnTryDamage += BaseObject_OnTryDamage;

        IEnumerable<StatusEffect> effectSource = baseObject is Entity ? (baseObject as Entity).StatusEffects
                                                                      : null;
        runtimeHP = defaultHPAttributes.RuntimeClone(effectSource);
    }

    /// <summary>
    /// Make an entity invulnerable for an unlimited time, until turned back off;
    /// </summary>
    /// <param name="on"> True makes the object invulnerable, false makes it vulnerable; </param>
    public void ToggleIFrame(bool on) {
        StopAllCoroutines();
        baseObject.ResetMaterials();
        iFrameOn = on;
    }

    private bool BaseObject_OnTryDamage(int amount, ElementType element) {
        if (!iFrameOn) {
            int processedAmount = runtimeHP.DoDamage(amount);
            OnDamageTaken?.Invoke(processedAmount);
            StartCoroutine(ISimulateIFrame());

            if (runtimeHP.Health <= 0) {
                baseObject.Perish();
                ToggleIFrame(true);
                return true;
            }
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