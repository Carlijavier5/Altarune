using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Pushable Module Section;
public abstract partial class BaseObject {

    public event System.Action<Vector3, EventResponse> OnTryFramePush;
    public event System.Action<Vector3, float, LongPushResponse> OnTryLongPush;

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
        LongPushResponse response = new();
        OnTryLongPush?.Invoke(direction * strength, duration, response);
        actionCore = response.actionCore;
        return actionCore != null;
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

public class LongPushResponse { public PushActionCore actionCore; }

public class PushActionCore {

    public AnimationCurve EaseCurve { get; private set; }
    private readonly DefaultEaseCurves defaultCurves;

    private readonly Pushable pushable;
    public readonly Vector3 direction;
    public readonly float duration;

    private float lifetime;

    public PushActionCore(Pushable pushable, Vector3 direction,
                          float duration, DefaultEaseCurves defaultCurves) {
        this.pushable = pushable;
        this.direction = direction;
        this.duration = duration;
        this.defaultCurves = defaultCurves;
        EaseCurve = defaultCurves.GetCurve(global::EaseCurve.Fixed);
    }

    public PushActionCore SetEase(EaseCurve easeCurve) {
        EaseCurve = defaultCurves.GetCurve(easeCurve);
        return this;
    }

    public PushActionCore SetEase(AnimationCurve easeCurve) {
        EaseCurve = easeCurve;
        return this;
    }

    public float UpdateLifetime(float deltaTime) {
        lifetime += deltaTime;
        return Mathf.Clamp01(lifetime / duration);
    }

    public void Kill() {
        if (pushable) pushable.RemoveCore(this);
    }
}