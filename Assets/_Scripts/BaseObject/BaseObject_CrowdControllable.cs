using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Crowd Controllable Module Section;
public abstract partial class BaseObject {

    public event System.Action<float, EventResponse> OnTryStagger;

    public virtual void Stun() { }
    public virtual void Restore() { }

    public bool TryStagger(float duration) {
        EventResponse response = new();
        OnTryStagger?.Invoke(duration, response);
        return response.received;
    }
}