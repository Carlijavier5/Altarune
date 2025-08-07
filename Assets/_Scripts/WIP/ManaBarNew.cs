using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarNew : MonoBehaviour {
    [SerializeField] private ManaSource manaSource;
    [SerializeField] private Image topLayer, interpolationLayer;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform anchor;
    [SerializeField] private AnimationCurve pulseAnimation;
    [SerializeField] private float fadeDuration, shakeAmountMult,
                                   shakeSpeedMult;

    private float Health => manaSource.Mana;
    private float MaxHealth => manaSource.MaxMana;
    private float FillAmount => MaxHealth > 0 ? Health / MaxHealth : 0;

    private Vector3 initialPosition;

    void Awake() {
        initialPosition = anchor.localPosition;
        manaSource.OnManaFill += AttachedEntity_OnHealReceived;
        manaSource.OnManaDrain += AttachedEntity_OnDamageTaken;
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

    private void AttachedEntity_OnDamageTaken(float amount) {
        topLayer.fillAmount = Health / MaxHealth;
    }

    private void AttachedEntity_OnHealReceived(float _) {
        interpolationLayer.fillAmount = Health / MaxHealth;
    }
}
