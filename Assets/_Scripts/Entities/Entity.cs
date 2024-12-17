using System.Collections.Generic;
using UnityEngine;

public enum EntityFaction { Friendly, Neutral, Hostile }

public abstract class Entity : BaseObject {

    [SerializeField] protected EntityFaction faction;
    public EntityFaction Faction => faction;

    public event System.Action<EntityEffect> OnEffectApplied;

    public HashSet<EntityEffect> StatusEffects { get; private set; } = new();
    private readonly Stack<EntityEffect> terminateStack = new();

    protected virtual void Update() {
        foreach (EntityEffect statusEffect in StatusEffects) {
            if (statusEffect.Update(this)) {
                statusEffect.Terminate(this);
                terminateStack.Push(statusEffect);
            }
            if (Perished) break;
        }

        while (terminateStack.TryPop(out EntityEffect deprecatedEffect)) {
            StatusEffects.Remove(deprecatedEffect);
        }
    }

    public void ApplyEffects(EntityEffect[] incomingEffects) {
        foreach (EntityEffect statusEffect in incomingEffects) {
            bool isNew = StatusEffects.Add(statusEffect);
            statusEffect.Apply(this, isNew);
            OnEffectApplied?.Invoke(statusEffect);
            statusEffect.Start(this);
        }
    }

    public override void Perish(bool immediate = false) {
        base.Perish(immediate);
        foreach (EntityEffect statusEffect in StatusEffects) {
            statusEffect.Terminate(this);
        } StatusEffects.Clear();
    }
}