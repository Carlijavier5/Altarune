using System.Collections.Generic;
using UnityEngine;

public enum StunType { Concussion, Freeze }

public class CrowdControllable : ObjectModule {

    [SerializeField] private CCAttributes ccAttributes;
    private RuntimeCCAttributes runtimeProperties;

    private IEnumerable<StatusEffect> effectSource;
    private float updateTimer;

    private float slowMult = 1;
    private float staggerTimer;
    private int ccEffectCount;

    void Awake() {
        baseObject.OnTryStagger += BaseObject_OnTryStagger;

        if (baseObject is Entity) {
            Entity entity = baseObject as Entity;
            entity.OnEffectApplied += Entity_OnEffectApplied;
            effectSource = entity.StatusEffects;
        }

        runtimeProperties = ccAttributes.RuntimeClone(effectSource);
    }

    void Update() {
        staggerTimer = Mathf.MoveTowards(staggerTimer, 0, Time.deltaTime);
        float staggerMult = staggerTimer > 0 ? 0 : 1;

        if (effectSource != null) {
            updateTimer += Time.deltaTime;
            if (updateTimer >= runtimeProperties.CCUpdateFrequency) {
                CCDataComposer dataComposer = new();
                foreach (StatusEffect statusEffect in effectSource) {
                    if (statusEffect.CCEffects != null) {
                        statusEffect.CCEffects.Update(updateTimer);
                        dataComposer.Compose(statusEffect.CCEffects);
                    }
                }

                baseObject.IsStunned = dataComposer.hasStun;
                baseObject.CanMove = !dataComposer.hasRoot;
                slowMult = dataComposer.slowMult;
                ccEffectCount = dataComposer.ccCount;

                updateTimer = 0;
            }
        }

        baseObject.TimeScale = slowMult * staggerMult;
    }

    private void BaseObject_OnTryStagger(float duration, EventResponse response) {
        staggerTimer = Mathf.Max(staggerTimer, duration);
        response.received = true;
    }

    private void Entity_OnEffectApplied(StatusEffect statusEffect) {
        runtimeProperties.ApplyAttributes(statusEffect.CCEffects, ccEffectCount);
    }

    private class CCDataComposer {
        public int ccCount;
        public bool hasStun, hasRoot;
        public float slowMult = 1;

        public void Compose(CrowdControlEffects cce) {
            bool hasStun = false;
            bool hasRoot = false;

            if (cce.stun != null && cce.stun.IsActive) {
                this.hasStun = true;
                hasStun = true;
            }

            if (cce.root != null && cce.root.IsActive) {
                this.hasRoot = true;
                hasRoot = true;
            }

            if (cce.slow != null && cce.slow.IsActive) {
                slowMult *= cce.slow.Multiplier;
            }

            if (hasStun || hasRoot) ccCount++;
        }
    }

    #if UNITY_EDITOR
    protected override void Reset() {
        base.Reset();
        if (CJUtils.AssetUtils.TryRetrieveAsset(out DefaultCrowdControlSettings settings)) {
            ccAttributes = new(settings);
        }
    }
    #endif
}