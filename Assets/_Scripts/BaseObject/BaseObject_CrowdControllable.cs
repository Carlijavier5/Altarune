using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Crowd Controllable Module Section;
public abstract partial class BaseObject {

    public class CCStatus {
        public float timeScale = 1;
        public bool isGrounded = true,
                    canMove = true,
                    isStunned;
    }
    
    public Vector3 LastGroundPoint { get; private set; }

    /// <summary>
    /// Crowd control status of the object; 
    /// </summary>
    public CCStatus Status { get; protected set; } = new();

    /// <summary>
    /// Subscribe to this event to react to slows/speed-ups; <br>
    /// </br> Argument is the value of status.timeScale;
    /// </summary>
    public event System.Action<float> OnTimeScaleSet;

    public float TimeScale {
        get => Status.timeScale;
        set {
            if (Status.timeScale != value) {
                Status.timeScale = value;
                OnTimeScaleSet?.Invoke(value);
            }
        }
    }

    /// <summary>
    /// Subscribe to this event to react to stuns; <br>
    /// </br> Argument is the value of status.isStunned;
    /// </summary>
    public event System.Action<bool> OnStunSet;

    public bool IsStunned {
        get => Status.isStunned;
        set {
            if (Status.isStunned != value) {
                Status.isStunned = value;
                OnStunSet?.Invoke(value);
            }
        }
    }

    /// <summary>
    /// Subscribe to this event to react to roots; <br>
    /// </br> Argument is the value of status.canMove;
    /// </summary>
    public event System.Action<bool> OnRootSet;

    public bool CanMove {
        get => Status.canMove;
        set {
            if (Status.canMove != value) {
                Status.canMove = value;
                OnRootSet?.Invoke(value);
            }
        }
    }

    public float RootMult => CanMove ? 1 : 0;

    protected event System.Action<bool> OnGroundedSet;
    
    public bool IsGrounded {
        get => Status.isGrounded;
        set {
            if (Status.isGrounded != value) {
                Status.isGrounded = value;
                if (!value) LastGroundPoint = transform.position;
                OnGroundedSet?.Invoke(value);
            }
        }
    }

    /// <summary>
    /// Utilize this value instead of <b>Time.deltaTime</b>
    /// where the object's local time scale is relevant;
    /// </summary>
    public float DeltaTime => Time.deltaTime * Status.timeScale;

    /// <summary>
    /// Utilize this value instead of <b>Time.fixedDeltaTime</b>
    /// where the object's local time scale is relevant;
    /// </summary>
    public float FixedDeltaTime => Time.fixedDeltaTime * Status.timeScale;

    /// Staggers are exclusive to the CrowdControllable Module;
    /// Implement the local timescale if you want your object to be staggerable;

    public event System.Action<float, bool, EventResponse> OnTryStagger;

    public bool TryStagger(float duration, bool timeStop = false) {
        EventResponse response = new();
        OnTryStagger?.Invoke(duration, timeStop, response);
        return response.received;
    }
}