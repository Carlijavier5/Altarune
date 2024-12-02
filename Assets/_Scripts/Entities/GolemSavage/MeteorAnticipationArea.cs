using System.Collections;
using UnityEngine;

public class MeteorAnticipationArea : MonoBehaviour {

    [SerializeField] private MeteorAnticipationGraphic edgeDecal,
                                                       innerDecal;

    public void DoArea(float duration) {
        edgeDecal.transform.localScale = Vector3.one;
        innerDecal.transform.localScale = Vector3.zero;
        edgeDecal.DoFade(true);
        innerDecal.DoFade(true);
        StartCoroutine(IDoArea(duration));
    }

    private IEnumerator IDoArea(float duration) {
        float lerpVal, timer = 0;
        while (timer < duration) {
            timer = Mathf.MoveTowards(timer, duration, Time.deltaTime);
            lerpVal = timer / duration;
            innerDecal.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerpVal);
            yield return null;
        }
        Toggle(false);
    }

    public void CancelArea() {
        StopAllCoroutines();
        Toggle(false);
    }

    private void Toggle(bool on) {
        edgeDecal.DoFade(on);
        innerDecal.DoFade(on);
    }
}