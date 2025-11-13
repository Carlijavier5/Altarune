using System.Collections;
using System.Threading;
using UnityEngine;

public class MeteorAnticipationArea : MonoBehaviour {

    [SerializeField] private MeteorAnticipationGraphic decal;
    [SerializeField] private float growTime = 0.5f;
    private Vector3 baseScale;

    public void Init(Transform anchor) {
        transform.SetParent(anchor);
    }

    public void DoArea(Vector3 position, Vector3 targetScale, float duration) {
        decal.transform.position = position;
        decal.transform.localScale = Vector3.zero;
        decal.DoFade(true);
        StartCoroutine(IDoCallbackOnTimer(duration, () => StartCoroutine(IDoArea(targetScale))));
    }

    private IEnumerator IDoArea(Vector3 targetSize) {
        float lerpVal = 0;
        while (Vector3.Distance(decal.transform.localScale, targetSize) > 0) {
            lerpVal = Mathf.MoveTowards(lerpVal, 1, growTime == 0 ? Mathf.Infinity : (Time.deltaTime / growTime));
            decal.transform.localScale = Vector3.Lerp(Vector3.zero, targetSize, lerpVal);
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
        decal.DoFade(on);
    }
}