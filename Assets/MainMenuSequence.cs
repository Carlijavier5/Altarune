using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MainMenuSequence : MonoBehaviour {
    [SerializeField] private CanvasGroup hide;
    [SerializeField] private CanvasGroup logo;
    [SerializeField] private CanvasGroup start;
    [SerializeField] private CanvasGroup credits;
    [SerializeField] private CanvasGroup quit;
    void Start() {
        StartCoroutine(StartAction());
    }

    private IEnumerator StartAction() {
        hide.DOFade(0f, 2f);
        yield return new WaitForSecondsRealtime(3f);
        logo.DOFade(1f, 3f);
        yield return new WaitForSecondsRealtime(2f);
        start.DOFade(1f, 1f);
        yield return new WaitForSecondsRealtime(0.3f);
        credits.DOFade(1f, 1f);
        yield return new WaitForSecondsRealtime(0.3f);
        quit.DOFade(1f, 1f);
    }
}
