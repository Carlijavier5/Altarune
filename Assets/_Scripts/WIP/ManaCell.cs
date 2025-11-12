using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaCell : MonoBehaviour {

    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup activationCG;
    [SerializeField] private ParticleSystem fillParticles, depleteParticles;
    private RectTransform RectTransform => transform as RectTransform;

    public void SetAnchors(float xMin, float xMax) {
        SetXMin(xMin);
        SetXMax(xMax);
    }

    public void DoCharge() {
        StopAllCoroutines();
        StartCoroutine(IDoCharge(1));
    }

    public void DoDischarge() {
        StopAllCoroutines();
        StartCoroutine(IDoCharge(0));
    }

    private IEnumerator IDoCharge(float target) {
        ParticleSystem activeSystem = target == 1 ? fillParticles : depleteParticles;
        activeSystem.Play();
        while (Mathf.Abs(activationCG.alpha - target) > Mathf.Epsilon) {
            activationCG.alpha = Mathf.MoveTowards(activationCG.alpha, target, Time.deltaTime * (target == 1 ? 1.5f : 3));
            yield return null;
        }
    }

    private void SetXMin(float xMin) {
        RectTransform.anchorMin = new(xMin, RectTransform.anchorMin.y);
    }

    private void SetXMax(float xMax) {
        RectTransform.anchorMax = new(xMax, RectTransform.anchorMax.y);
    }
}
