using System.Collections;
using UnityEngine;

public class SovereignLaserWarning : MonoBehaviour {

    public event System.Action OnWarningFinished;

    [SerializeField] private GraphicFader warning;

    public void DoWarning(float warningTime) {
        StopAllCoroutines();
        StartCoroutine(IAnimateWarning(warningTime));
    }

    private IEnumerator IAnimateWarning(float warningTime) {
        warning.DoFade(true);

        float warnTimer = 0;
        while (warnTimer < warningTime) {
            warnTimer = Mathf.MoveTowards(warnTimer, warningTime, Time.deltaTime);
            yield return null;
        }

        OnWarningFinished?.Invoke();
        OnWarningFinished = null;
        warning.DoFade(false);
    }
}