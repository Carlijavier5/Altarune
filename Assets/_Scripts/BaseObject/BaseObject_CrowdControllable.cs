using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Crowd Controllable Module Section;
public abstract partial class BaseObject {

    protected class CCStatus {
        public float timeScale = 1;
        public bool isGrounded = true,
                    canMove = true,
                    isStunned;
    }
    
    public Vector3 LastGroundPoint { get; private set; }

    /// <summary>
    /// Crowd control status of the object; 
    /// </summary>
    protected CCStatus status = new();

    /// <summary>
    /// Subscribe to this event to react to slows/speed-ups; <br>
    /// </br> Argument is the value of status.timeScale;
    /// </summary>
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

    /// <summary>
    /// Subscribe to this event to react to stuns; <br>
    /// </br> Argument is the value of status.isStunned;
    /// </summary>
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

    /// <summary>
    /// Subscribe to this event to react to roots; <br>
    /// </br> Argument is the value of status.canMove;
    /// </summary>
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

    protected event System.Action<bool> OnGroundedSet;
    
    public bool IsGrounded {
        get => status.isGrounded;
        set {
            if (status.isGrounded != value) {
                status.isGrounded = value;
                if (!value) LastGroundPoint = transform.position;
                OnGroundedSet?.Invoke(value);
            }
        }
    }

    /// <summary>
    /// Utilize this value instead of <b>Time.deltaTime</b>
    /// where the object's local time scale is relevant;
    /// </summary>
    public float DeltaTime => Time.deltaTime * status.timeScale;

    /// <summary>
    /// Utilize this value instead of <b>Time.fixedDeltaTime</b>
    /// where the object's local time scale is relevant;
    /// </summary>
    public float FixedDeltaTime => Time.fixedDeltaTime * status.timeScale;

    /// Staggers are exclusive to the CrowdControllable Module;
    /// Implement the local timescale if you want your object to be staggerable;

    public event System.Action<float, EventResponse> OnTryStagger;

    public bool TryStagger(float duration) {
        EventResponse response = new();
        OnTryStagger?.Invoke(duration, response);
        return response.received;
    }
}