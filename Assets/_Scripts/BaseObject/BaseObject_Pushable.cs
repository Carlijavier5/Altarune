using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Pushable Module Section;
public abstract partial class BaseObject {

    public event System.Action<Vector3, EventResponse> OnTryFramePush;
    public event System.Action<Vector3, float, EventResponse<PushActionCore>> OnTryLongPush;

    public MotionDriver MotionDriver { get; private set; } = new();

    /// <summary>
    /// Push the object within this frame; <br/>
    /// Should be called over several frames for visible effects;
    /// </summary>
    /// <returns> True if the object was pushed, false otherwise; </returns>
    public bool TryPush(Vector3 direction, float strength) {
        EventResponse response = new();
        OnTryFramePush?.Invoke(direction.normalized * strength, response);
        return response.received;
    }

    /// <summary>
    /// Runs a push coroutine on the object with a set duration;
    /// </summary>
    /// <param name="duration"> Duration, in seconds, of the push action; </param>
    /// <returns> True if the object was pushed, false otherwise; </returns>
    public bool TryLongPush(Vector3 direction, float strength,
                            float duration, out PushActionCore actionCore) {
        EventResponse<PushActionCore> response = new();
        OnTryLongPush?.Invoke(direction.normalized * strength, duration, response);
        actionCore = response.objectReference;
        return response.received;
    }

    /// <summary>
    /// Runs a push coroutine on the object with a set duration;
    /// </summary>
    /// <param name="duration"> Duration, in seconds, of the push action; </param>
    /// <returns> True if the object was pushed, false otherwise; </returns>
    public bool TryLongPush(Vector3 direction, float strength, float duration) {
        return TryLongPush(direction, strength, duration, out _);
    }
}