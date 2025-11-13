using System.Collections;
using UnityEngine;

public class PingPongFadeAnimator : MonoBehaviour {

    [SerializeField] private CanvasGroup canvasGroup;
    [Tooltip("Minimum fade the fill fade highlight will ping-pong from;")]
    [SerializeField] private float minFadeAlpha = 0.1f;
    [Tooltip("How fast does the image fill move towards the target alpha;")]
    [SerializeField] private float fillFadeSpeed = 1;
    [Tooltip("Ping-pong interval length for the fill fade, higher values result in shorter intervals;")]
    [SerializeField] private float fillFadeInterval = 1.5f;
    [Tooltip("Fade speed multiplier for when the cell highlight fade must end;")]
    [SerializeField] private float fillFadeEndSpeedMult = 2;

    public void DoFade(bool on) {
        StopAllCoroutines();
        StartCoroutine(IDoFade(on));
    }

    private IEnumerator IDoFade(bool on) {
        float normAlpha = Mathf.InverseLerp(minFadeAlpha, 1, canvasGroup.alpha);
        float ppTimeUpCycle = Mathf.Repeat(Time.time * fillFadeInterval, 2);

        bool isRising = ppTimeUpCycle < 1;

        float phaseShift = isRising ? Mathf.Repeat(normAlpha - ppTimeUpCycle, 2)
                                    : Mathf.Repeat(2 - normAlpha - ppTimeUpCycle, 2);
        float lerpVal, fadeTarget;
        while (true) {
            lerpVal = Mathf.PingPong(Time.time * fillFadeInterval + phaseShift, 1);
            fadeTarget = on ? Mathf.Lerp(minFadeAlpha, 1, lerpVal) : 0;
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, fadeTarget, Time.deltaTime * fillFadeSpeed * (on ? 1 : fillFadeEndSpeedMult));
            if (!on && canvasGroup.alpha == fadeTarget) break;
            yield return null;
        }
    }
}