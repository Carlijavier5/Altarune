using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour {

    public event System.Action OnFadeEnd;

    [SerializeField] private Image image;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float fadeTime = 1;
    private float timer = 1;

    /// <summary>
    /// Uncover screen;
    /// </summary>
    public void FadeIn(bool immediateFade = false) {
        DoFade(false, immediateFade);
    }

    /// <summary>
    /// Cover screen;
    /// </summary>
    public void FadeOut(bool immediateFade = false) {
        DoFade(true, immediateFade);
    }

    private void DoFade(bool on, bool immediateFade) {
        StopAllCoroutines();
        if (immediateFade) {
            float alpha = on ? 1 : 0;
            image.color = new Color(image.color.r, image.color.g,
                                    image.color.b, alpha);
        } else {
            StartCoroutine(IDoFade(on));
        }
    }

    private IEnumerator IDoFade(bool on) {
        float alpha, lerpVal,
              target = on ? fadeTime : 0;
        while (Mathf.Abs(timer - target) > 0) {
            timer = Mathf.MoveTowards(timer, target, Time.unscaledDeltaTime);
            lerpVal = timer / fadeTime;
            alpha = curve.Evaluate(lerpVal);
            image.color = new Color(image.color.r, image.color.g,
                                    image.color.g, alpha);
            yield return null;
        }

        OnFadeEnd?.Invoke();
    }
}