using System.Collections;
using UnityEngine;

public class CanvasGroupFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] protected float fadeTime;

    public void DoFade(bool on) {
        StopAllCoroutines();
        StartCoroutine(IDoFade(on));
    }

    public IEnumerator IDoFade(bool on) {
        float target = on ? 1 : 0;
        while (Mathf.Abs(target - canvasGroup.alpha) > 0) {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target,
                                                  fadeTime == 0 ? Mathf.Infinity : Time.unscaledDeltaTime / fadeTime);
            yield return null;
        }
    }
}
