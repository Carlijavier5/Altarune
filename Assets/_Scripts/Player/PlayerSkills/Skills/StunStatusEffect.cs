using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunStatusEffect : EntityEffect
{
    [SerializeField] private float duration;

    public StunStatusEffect(float duration) { this.duration = duration; }

    public override void Apply(Entity entity, bool isNew) {
        CCEffects = new() { root = new() { duration = duration } };
    }

    public override void Terminate(Entity entity) { }

    public override bool Update(Entity entity) {
        duration -= Time.deltaTime;

        return duration <= 0;
    }
}
