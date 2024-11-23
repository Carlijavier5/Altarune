using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private BaseObject attachedEntity;
    [SerializeField] private Image topLayer, interpolationLayer;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform anchor;
    [SerializeField] private AnimationCurve pulseAnimation;
    [SerializeField] private float fadeDuration,
                                   shakeAmountMult, shakeSpeedMult;

    private float Health => attachedEntity.Health;
    private float MaxHealth => attachedEntity.MaxHealth;
    private float FillAmount => MaxHealth > 0 ? Health / MaxHealth : 0;

    private Vector3 initialPosition;
    private float timer;
    private bool active;

    void Awake() {
        initialPosition = anchor.localPosition;
        attachedEntity.OnHealingReceived += AttachedEntity_OnHealReceived;
        attachedEntity.OnDamageReceived += AttachedEntity_OnDamageTaken;
        attachedEntity.OnPerish += AttachedEntity_OnPerish;
    }

    void Update() {
        topLayer.fillAmount = Mathf.MoveTowards(topLayer.fillAmount,
                                                FillAmount, Time.deltaTime);
        interpolationLayer.fillAmount = Mathf.MoveTowards(interpolationLayer.fillAmount,
                                                          FillAmount, Time.deltaTime);
        Shaking();
    }

    private void Shaking() {
        float healthDiff = interpolationLayer.fillAmount - Health / MaxHealth;
        if (healthDiff > 0) {
            float diffPercent = healthDiff / MaxHealth;
            float shakeAmount = diffPercent * shakeAmountMult;
            float shakeSpeed = diffPercent * shakeSpeedMult;

            anchor.localPosition = initialPosition + new Vector3(Mathf.Sin(Time.time * shakeSpeed) * shakeAmount, 0,
                                                                 Mathf.Sin(Time.time * shakeSpeed) * shakeAmount);
        } else {
            anchor.localPosition = initialPosition;
        }
    }

    private IEnumerator ToggleBar(bool on) {
        float target = on ? 1 : 0;
        float lerpVal;
        while (timer != target) {
            timer = Mathf.MoveTowards(timer, target, Time.deltaTime);
            lerpVal = timer / fadeDuration;

            transform.localScale = Vector3.one * pulseAnimation.Evaluate(lerpVal);
            canvasGroup.alpha = Mathf.Lerp(0, 1, lerpVal);
            yield return null;
        }
    }

    private void AttachedEntity_OnDamageTaken(int _) {
        topLayer.fillAmount = Health / MaxHealth;
        if (Health > 0 && !active) {
            StartCoroutine(ToggleBar(true));
            active = true;
        }
    }

    private void AttachedEntity_OnHealReceived(int _) {
        interpolationLayer.fillAmount = Health / MaxHealth;
    }

    private void AttachedEntity_OnPerish(BaseObject _) {
        StopAllCoroutines();
        StartCoroutine(ToggleBar(false));
    }
}
