using UnityEngine;

public class PushActionCore {

    public AnimationCurve EaseCurve { get; private set; }
    private readonly DefaultEaseCurves defaultCurves;

    private readonly Pushable pushable;
    public readonly Vector3 direction;
    public readonly float duration;

    public Vector3 ValueAtLifetime => direction * EaseCurve.Evaluate(lifetime);
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