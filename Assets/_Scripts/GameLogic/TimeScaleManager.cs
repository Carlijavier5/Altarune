using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour {

    private readonly HashSet<TimeScaleCore> coreSet = new();
    private readonly Stack<TimeScaleCore> terminateStack = new();

    private float globalTimeScale = 1;
    public float GlobalTimeScale {
        get => globalTimeScale;
        set {
            globalTimeScale = Mathf.Clamp(value, 0, 2);
        }
    }

    void Update() {
        float timeScale = globalTimeScale;
        foreach (TimeScaleCore core in coreSet) {
            if (core.Update()) terminateStack.Push(core);
            timeScale *= core.TimeScale;
        }
        while (terminateStack.TryPop(out TimeScaleCore core)) {
            coreSet.Remove(core);
        }
        Time.timeScale = timeScale;
    }

    /// <summary>
    /// Add a timescale multiplier 'core' to the manager;
    /// </summary>
    /// <param name="timeScale"> A value from 0 to 2; </param>
    /// <param name="duration"> Duration of the timescale shift; </param>
    /// <param name="evoCurve"> Optional animation curve to dictate the fluctuation of the multiplier; <br>
    /// </br> The values contained should be constrained between 0 and 1; </param>
    /// <returns> A timescale 'core' that can be killed and modified on command; </returns>
    public TimeScaleCore AddTimeScaleMultiplier(float timeScale, float duration,
                                                AnimationCurve evoCurve = null, bool isPermanent = false) {
        TimeScaleCore core = new(timeScale, duration, evoCurve, isPermanent);
        coreSet.Add(core);
        return core;
    }
}

/// <summary>
/// An object living in the TimeScaleManager, which contributes
/// an impermanent multiplier to the global timescale; <br>
/// </br> You may modify its timer (time elapsed since creation)
/// and duration (maximum time until expiration) freely; <br>
/// </br> You may 'kill' the core to remove it from the global set
/// (will take effect on the next frame);<br>
/// </br> Should you need to modify the core's timescale value directly,
/// you should 'kill' it and request a new one instead;<br>
/// </br> After a core expires, it is removed from the global set
/// and any changes to it will have no effect on the global timescale;
/// </summary>
public class TimeScaleCore {

    public float TimeScale { get; private set; }
    public float timer, duration;

    public AnimationCurve evoCurve;
    public bool IsExpired { get; private set; }

    private readonly bool isPermanent;
    private readonly float timeScaleShift;

    public TimeScaleCore(float timeScale, float duration,
                         AnimationCurve evoCurve = null, bool isPermanent = false) {
        TimeScale = timeScale;
        this.duration = duration;
        this.evoCurve = evoCurve;
        this.isPermanent = isPermanent;
        timeScaleShift = timeScale - 1;
    }

    public bool Update() {
        timer = Mathf.MoveTowards(timer, duration, Time.unscaledDeltaTime);
        if (evoCurve != null) {
            float lerpVal = timer / duration;
            TimeScale = Mathf.Clamp(1 + evoCurve.Evaluate(lerpVal) * timeScaleShift, 0, 2);
        }
        if (timer >= duration && !isPermanent) IsExpired = true;
        return IsExpired;
    }

    public void Kill() => IsExpired = true;
}