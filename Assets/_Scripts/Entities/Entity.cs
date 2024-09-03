using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : BaseObject {

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

public abstract class StatusEffect {
    /// <summary>
    /// Called when the effect gets applied;
    /// </summary>
    public abstract void Apply(Entity entity);
    /// <summary>
    /// Called from the holding entity's update thread;
    /// </summary>
    /// <returns> True if the effect should be returned, false otherwise; </returns>
    public abstract bool Update(Entity entity);
    /// <summary>
    /// Called when the effect is removed;
    /// </summary>
    public abstract void Terminate(Entity entity);
}