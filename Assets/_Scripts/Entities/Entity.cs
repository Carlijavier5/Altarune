using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityFaction { Friendly, Neutral, Hostile }

public class Entity : BaseObject {

    [SerializeField] private EntityFaction faction;
    public EntityFaction Faction => faction;

    public HashSet<StatusEffect> StatusEffects { get; private set; } = new();

    protected virtual void Update() {
        foreach (StatusEffect statusEffect in StatusEffects) {
            if (statusEffect.Update(this)) {
                statusEffect.Terminate(this);
                StatusEffects.Remove(statusEffect);
            }
        }
    }

    private void ApplyEffects(StatusEffect[] incomingEffects) {
        foreach (StatusEffect statusEffect in incomingEffects) {
            StatusEffects.Add(statusEffect);
            statusEffect.Apply(this);
        }
    }
}