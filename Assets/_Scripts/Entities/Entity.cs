using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityFaction { Friendly, Neutral, Hostile }

public class Entity : BaseObject {

    [SerializeField] private EntityFaction faction;
    public EntityFaction Faction => faction;

    public HashSet<StatusEffect> StatusEffects { get; private set; } = new();
    private readonly Stack<StatusEffect> terminateStack = new();

    protected virtual void Update() {
        foreach (StatusEffect statusEffect in StatusEffects) {
            if (statusEffect.Update(this)) {
                statusEffect.Terminate(this);
                terminateStack.Push(statusEffect);
            }
            if (Perished) break;
        }

        while (terminateStack.TryPop(out StatusEffect deprecatedEffect)) {
            StatusEffects.Remove(deprecatedEffect);
        }
    }

    public void ApplyEffects(StatusEffect[] incomingEffects) {
        foreach (StatusEffect statusEffect in incomingEffects) {
            bool isNew = StatusEffects.Add(statusEffect);
            statusEffect.Apply(this, isNew);
        }
    }

    public override void Perish() {
        base.Perish();
        foreach (StatusEffect statusEffect in StatusEffects) {
            statusEffect.Terminate(this);
        } StatusEffects.Clear();
    }
}