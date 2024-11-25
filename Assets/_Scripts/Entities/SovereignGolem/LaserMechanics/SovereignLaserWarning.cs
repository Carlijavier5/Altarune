using System.Collections;
using UnityEngine;

public class SovereignLaserWarning : MonoBehaviour {

    public event System.Action OnWarningFinished;

    [SerializeField] private GameObject warning;

    public void DoWarning(float warningTime) {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(IAnimateWarning(warningTime));
    }

    private IEnumerator IAnimateWarning(float warningTime) {
        warning.SetActive(true);

        float warnTimer = 0;
        while (warnTimer < warningTime) {
            warnTimer = Mathf.MoveTowards(warnTimer, warningTime, Time.deltaTime);
            yield return null;
        }

        OnWarningFinished?.Invoke();
        OnWarningFinished = null;
        warning.SetActive(false);

        gameObject.SetActive(false);
    }
}