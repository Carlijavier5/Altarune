using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Crowd Controllable Module Section;
public abstract partial class BaseObject {

    protected class CCStatus {
        public float timeScale = 1;
        public bool isGrounded, isStunned,
                    canMove = true;
    } protected CCStatus status = new();

    protected event System.Action<float> OnTimeScaleSet;

    public float TimeScale {
        get => status.timeScale;
        set {
            if (status.timeScale != value) {
                status.timeScale = value;
                OnTimeScaleSet?.Invoke(value);
            }
        }
    }

    protected event System.Action<bool> OnStunSet;

    public bool IsStunned {
        get => status.isStunned;
        set {
            if (status.isStunned != value) {
                status.isStunned = value;
                OnStunSet?.Invoke(value);
            }
        }
    }

    protected event System.Action<bool> OnRootSet;

    public bool CanMove {
        get => status.canMove;
        set {
            if (status.canMove != value) {
                status.canMove = value;
                OnRootSet?.Invoke(value);
            }
        }
    }

    protected float DeltaTime { get { return Time.deltaTime * status.timeScale; } }

    public event System.Action<float, EventResponse> OnTryStagger;

    public bool TryStagger(float duration) {
        EventResponse response = new();
        OnTryStagger?.Invoke(duration, response);
        return response.received;
    }
}