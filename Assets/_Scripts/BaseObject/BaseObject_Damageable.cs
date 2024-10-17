using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Damageable Module Section;
public abstract partial class BaseObject {

    public event System.Action<int, ElementType, EventResponse> OnTryDamage;
    public event System.Action<EventResponse<int>> OnTryRequestHealth;

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

    public bool Perished { get; private set; }

    /// <summary>
    /// Override to implement a death behavior for the object; <br/>
    /// </summary>
    public virtual void Perish() {
        Perished = true;
        OnPerish?.Invoke(this);
    }

    /// <summary>
    /// Damage method, attempts to damage the object;
    /// </summary>
    /// <returns> True if the object is damageable; </returns>
    public bool TryDamage(int amount, ElementType element = ElementType.Physical) {
        EventResponse eRes = new();
        OnTryDamage?.Invoke(amount, element, new());
        return eRes.received;
    }
}
