using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType { Physical, Fire, Ice, Shock, Poison }

public class Damageable : ObjectModule {

    public event System.Action OnModuleInit;

    [SerializeField] protected HealthAttributes defaultHPAttributes;
    [SerializeField] protected IFrameProperties iFrameProperties;

    public int Health => runtimeHP != null ? runtimeHP.Health
                                           : -1;

    protected RuntimeHealthAttributes runtimeHP;

    protected bool localIFrameOn, externalIFrameOn;
    protected bool IFrameOn => localIFrameOn || externalIFrameOn;

    void Awake() {
        baseObject.UpdateRendererRefs();
        baseObject.OnTryDamage += BaseObject_OnTryDamage;
        baseObject.OnTryRequestHealth += BaseObject_OnTryRequestHealth;
        baseObject.OnTryToggleIFrame += BaseObject_OnTryToggleIFrame;

        IEnumerable<StatusEffect> effectSource = baseObject is Entity ? (baseObject as Entity).StatusEffects
                                                                      : null;
        runtimeHP = defaultHPAttributes.RuntimeClone(effectSource);

        OnModuleInit?.Invoke();
    }

    private void BaseObject_OnTryToggleIFrame(bool on, EventResponse response) {
        response.received = true;
        ToggleIFrame(on);
    }

    private void BaseObject_OnTryRequestHealth(EventResponse<int> response) {
        response.received = true;
        response.objectReference = Health;
    }

    /// <summary>
    /// Make an entity invulnerable for an unlimited time, until turned back off;
    /// </summary>
    /// <param name="on"> True makes the object invulnerable, false makes it vulnerable; </param>
    public void ToggleIFrame(bool on) {
        externalIFrameOn = on;
    }

    protected virtual void BaseObject_OnTryDamage(int amount, ElementType element,
                                                  EventResponse response) {
        if (!IFrameOn) {
            response.received = true;

            int processedAmount = runtimeHP.DoDamage(amount);
            if (processedAmount > 0) {
                baseObject.PropagateDamage(processedAmount);
                StartCoroutine(ISimulateIFrame());

                if (runtimeHP.Health <= 0) {
                    baseObject.Perish();
                    ToggleIFrame(true);
                }
            }
        }
    }

    protected virtual IEnumerator ISimulateIFrame() {
        localIFrameOn = true;
        baseObject.SetMaterial(iFrameProperties.settings.flashMaterial);
        yield return new WaitForSeconds(iFrameProperties.duration);
        baseObject.ResetMaterials();
        localIFrameOn = false;
    }

    #if UNITY_EDITOR
    protected override void Reset() {
        base.Reset();
        if (CJUtils.AssetUtils.TryRetrieveAsset(out DefaultHealthAttributeCurves curves)) {
            defaultHPAttributes = new(curves);
        }
        if (CJUtils.AssetUtils.TryRetrieveAsset(out DefaultIFrameProperties properties)) {
            iFrameProperties = properties.DefaultProperties;
        }
    }
    #endif
}