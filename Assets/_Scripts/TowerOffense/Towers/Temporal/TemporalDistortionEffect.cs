using UnityEngine;

public class TemporalDistortionEffect : EntityEffect {

    private readonly float strength, maxDuration;
    private float effectDuration;

    public TemporalDistortionEffect(float strength, float duration) {
        this.strength = strength;
        maxDuration = duration;
        effectDuration = duration;
    }

    public override void Apply(Entity entity, bool isNew) {
        if (isNew) {
            CCEffects = new() { slow = new() { duration = effectDuration,
                                    Multiplier = strength,
                                    pierceRes = true } };
        } else {
            CCEffects.slow.duration = maxDuration;
            effectDuration = maxDuration;
        }
    }

    public override bool Update(Entity entity) {
        effectDuration -= Time.deltaTime;
        return effectDuration <= 0;
    }

    public override void Terminate(Entity entity) { }
}