using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaCell : MonoBehaviour {

    [SerializeField] private Image image;
    [SerializeField] private ParticleSystem particles;
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
        particles.Play();
        while (Mathf.Abs(image.fillAmount - target) > Mathf.Epsilon) {
            image.fillAmount = Mathf.MoveTowards(image.fillAmount, target, Time.deltaTime);
            yield return null;
        }
        particles.Stop();
    }

    private void SetXMin(float xMin) {
        RectTransform.anchorMin = new(xMin, RectTransform.anchorMin.y);
    }

    private void SetXMax(float xMax) {
        RectTransform.anchorMax = new(xMax, RectTransform.anchorMax.y);
    }
}
