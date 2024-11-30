using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour {

    private readonly LinkedList<TimeScaleCore> coreList = new();
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
        foreach (TimeScaleCore core in coreList) {
            if (core.Update()) terminateStack.Push(core);
            timeScale *= core.timeScale;
        }
        while (terminateStack.TryPop(out TimeScaleCore core)) {
            coreList.Remove(core);
        }
        Time.timeScale = timeScale;
    }

    public void AddTimeScaleShift(float timeScale, float duration,
                                  AnimationCurve evoCurve = null) {
        TimeScaleCore core = new(timeScale, duration, evoCurve);
        coreList.AddLast(core);
    }
}

public class TimeScaleCore {
    public float timeScaleShift, duration,
                 timeScale, timer;
    public AnimationCurve evoCurve;

    public TimeScaleCore(float timeScale, float duration,
                         AnimationCurve evoCurve = null) {
        this.timeScale = timeScale;
        this.duration = duration;
        this.evoCurve = evoCurve;
        timeScaleShift = timeScale - 1;
    }

    public bool Update() {
        timer = Mathf.MoveTowards(timer, duration, Time.unscaledDeltaTime);
        if (evoCurve != null) {
            float lerpVal = timer / duration;
            timeScale = Mathf.Clamp01(1 + evoCurve.Evaluate(lerpVal) 
                                          * timeScaleShift);
        } return timer >= duration;
    }
}