using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Damageable Module Section;
public abstract partial class BaseObject {

    public event System.Action<int, ElementType, EventResponse> OnTryDamage;
    public event System.Action<int, EventResponse> OnTryHeal;

    public event System.Action<bool, EventResponse> OnTryToggleIFrame;
    public event System.Action<int> OnTryBypassIFrame;

    public event System.Action<EventResponse<int>> OnTryRequestHealth;
    public event System.Action<EventResponse<int>> OnTryRequestMaxHealth;

    public event System.Action<int> OnDamageReceived;
    public event System.Action<int> OnHealingReceived;
    public event System.Action<BaseObject> OnPerish;

    /// <summary>
    /// Object health; <br/>
    /// • Returns the current health if damageable and alive; <br/>
    /// • Returns 0 if damageable and not alive; <br/>
    /// • Returns -1 if not damageable;
    /// </summary>
    public int Health {
        get {
            EventResponse<int> response = new();
            OnTryRequestHealth?.Invoke(response);
            return response.received ? response.objectReference : -1;
        }
    }

    /// <summary>
    /// Object max health; <br/>
    /// • Returns the current max health if damageable and alive; <br/>
    /// • Returns -1 if not damageable;
    /// </summary>
    public int MaxHealth {
        get {
            EventResponse<int> response = new();
            OnTryRequestMaxHealth?.Invoke(response);
            return response.received ? response.objectReference : -1;
        }
    }

    public bool Perished { get; private set; }

    /// <summary>
    /// Override to implement a death behavior for the object; <br/>
    /// </summary>
    public virtual void Perish() {
        Perished = true;
        OnPerish?.Invoke(this);
    }

    public void PropagateDamage(int amount) => OnDamageReceived?.Invoke(amount);

    public void PropagateHeal(int amount) => OnHealingReceived?.Invoke(amount);

    /// <summary>
    /// Damage method, attempts to damage the object;
    /// </summary>
    /// <returns> True if the object is damageable; </returns>
    public bool TryDamage(int amount, ElementType element = ElementType.Physical) {
        EventResponse eRes = new();
        OnTryDamage?.Invoke(amount, element, eRes);
        return eRes.received;
    }

    public void TryBypassIFrame(int damageAmount) {
        OnTryBypassIFrame?.Invoke(damageAmount);
    }

    /// <summary>
    /// Heal method, attempts to heal the object;
    /// </summary>
    /// <returns> True if the object is damageable; </returns>
    public bool TryHeal(int amount) {
        EventResponse eRes = new();
        OnTryHeal?.Invoke(amount, new());
        return eRes.received;
    }

    /// <summary>
    /// Toggle the external i-frame of this object;
    /// </summary>
    /// <returns> Whether said object is damageable; </returns>
    protected bool TryToggleIFrame(bool on) {
        EventResponse eRes = new();
        OnTryToggleIFrame?.Invoke(on, eRes);
        return eRes.received;
    }
}