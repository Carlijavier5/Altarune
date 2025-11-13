using System.Collections;
using System.Threading;
using UnityEngine;

public class MeteorAnticipationArea : MonoBehaviour {

    [SerializeField] private MeteorAnticipationGraphic edgeDecal,
                                                       innerDecal;
    [SerializeField] private float growTime = 0.5f;

    public void DoArea(float duration) {
        edgeDecal.transform.localScale = Vector3.one;
        innerDecal.transform.localScale = Vector3.zero;
        edgeDecal.DoFade(true);
        innerDecal.DoFade(true);
        StartCoroutine(IDoCallbackOnTimer(duration, () => StartCoroutine(IDoArea())));
    }

    private IEnumerator IDoArea() {
        float lerpVal = 0;
        while (Vector3.Distance(innerDecal.transform.localScale, Vector3.one) > 0) {
            lerpVal = Mathf.MoveTowards(lerpVal, 1, growTime == 0 ? Mathf.Infinity : (Time.deltaTime / growTime));
            innerDecal.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerpVal);
            yield return null;
        }
    }

    private IEnumerator IDoCallbackOnTimer(float duration, System.Action callback) {
        float timer = 0;
        while (Mathf.Abs(timer - duration) > 0) {
            timer = Mathf.MoveTowards(timer, duration, Time.deltaTime);
            yield return null;
        }

        callback.Invoke();
    }

    public void EndArea(float duration) {
        StopAllCoroutines();
        StartCoroutine(IDoCallbackOnTimer(duration, () => Toggle(false)));
    }

    public void EndAreaAbrupt() {
        StopAllCoroutines();
        Toggle(false);
    }

    private void Toggle(bool on) {
        edgeDecal.DoFade(on);
        innerDecal.DoFade(on);
    }
}