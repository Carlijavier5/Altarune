using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeCCAttributes : CCAttributes {

    private readonly CCAttributes ccAttributes;
    private IEnumerable<EntityEffect> effectSource;

    public RuntimeCCAttributes(CCAttributes defaultAttributes, DefaultCrowdControlSettings settings,
                               IEnumerable<EntityEffect> effectSource) : base(settings) {
        ccAttributes = defaultAttributes;
        this.effectSource = effectSource;
    }

    public void ApplyAttributes(CrowdControlEffects cce, int ccEffectCount) {
        if (cce != null) {
            UpdateAttributes();

            float ccDurationPenalty = ccSettings.ccDRCurve.Evaluate(ccEffectCount)
                                    * ccSettings.ccResCurve.Evaluate(ccResistance);

            if (cce.stun != null && !cce.stun.pierceRes) {
                cce.stun.duration *= ccSettings.stunResCurve.Evaluate(stunResistance) * ccDurationPenalty;
            }
            if (cce.root != null && !cce.root.pierceRes) {
                cce.root.duration *= ccSettings.rootResCurve.Evaluate(rootResistance) * ccDurationPenalty;
            }
            if (cce.slow != null && !cce.slow.pierceRes) {
                cce.slow.Multiplier = Mathf.Lerp(1, cce.slow.Multiplier,
                                                 ccSettings.slowResCurve.Evaluate(slowResistance));
            }
        }
    }

    private void UpdateAttributes() {
        if (effectSource == null) return;

        CCAttributeModifiers composite = new();
        foreach (EntityEffect statusEffect in effectSource) {
            if (statusEffect.HealthModifiers != null) {
                composite.Compose(statusEffect.CCModifiers);
            }
        }

        composite.ApplyModifiers(this, ccAttributes);
    }
}