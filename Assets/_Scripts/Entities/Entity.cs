using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityFaction { Friendly, Neutral, Hostile }

/// <summary>
/// Layer for status effect and attributes
/// </summary>
public class Entity : BaseObject {

    [SerializeField] private EntityFaction faction;
    public EntityFaction Faction => faction;

    protected HashSet<StatusEffect> statusEffects = new();

    protected virtual void Update() {
        foreach (StatusEffect statusEffect in statusEffects) {
            if (statusEffect.Update(this)) {
                statusEffect.Terminate(this);
                statusEffects.Remove(statusEffect);
            }
        }
    }

    private void ApplyEffects(StatusEffect[] incomingEffects) {
        foreach (StatusEffect statusEffect in incomingEffects) {
            statusEffects.Add(statusEffect);
            statusEffect.Apply(this);
        }
    }
}