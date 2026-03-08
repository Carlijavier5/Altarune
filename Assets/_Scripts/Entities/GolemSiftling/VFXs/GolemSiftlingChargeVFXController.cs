using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GolemSiftlingChargeVFXController : MonoBehaviour
{
    private const string PROPERTY_PRECHARGE_COL = "ColorOverLifetime";

    [SerializeField] private ParticleSystem prechargeWind;
    [SerializeField] private VisualEffect prechargeParticles;
    [SerializeField] private SiftlingDustChargeController chargeDustController;
    [SerializeField] private LoopingSystemController chargeShield;

    [SerializeField] private Vector2 prechargeWindYPositionRange, prechargeWindYScaleRange;
    [SerializeField] private Color prechargeWindStartColor, prechargeWindEndColor;
    [GradientUsage(true)][SerializeField] private Gradient prechargeWindStartGradient, prechargeWindEndGradient;
    [GradientUsage(true)][SerializeField] private Gradient prechargeParticlesStartGradient, prechargeParticlesEndGradient;

    [Tooltip("Time of the precharge duration the precharge effects will remain fully lerped.\n" +
             "Should be shorter than the total precharge duration.")]
    [SerializeField] private float prechargeCompletionDurationBuffer;

    private float prechargeStartTime, prechargeEndTime;

    private int propertyPrechargeCOL;

    void Awake() {
        prechargeParticles.Stop();
        propertyPrechargeCOL = Shader.PropertyToID(PROPERTY_PRECHARGE_COL);
    }

    public void DoPrecharge(float duration) {
        LerpPrecharge(0);
        prechargeStartTime = Time.time;
        prechargeEndTime = Time.time + duration - prechargeCompletionDurationBuffer;

        prechargeWind.Play();
        prechargeParticles.Play();

        StopAllCoroutines();
        StartCoroutine(IDoPrecharge());
    }

    public void DoCharge() {
        chargeShield.Enable();
        chargeDustController.Play();
    }

    public void CancelPrecharge() {
        StopAllCoroutines();
        prechargeWind.Stop();
        prechargeParticles.Stop();
        StartCoroutine(ICancelPrecharge());
    }

    public void CancelAll() {
        CancelPrecharge();
        CancelCharge();
    }

    private IEnumerator IDoPrecharge() {
        float lerpVal = 0;
        while (lerpVal < 1) {
            lerpVal = Mathf.Clamp01((Time.time - prechargeStartTime) / (prechargeEndTime - prechargeStartTime));
            LerpPrecharge(lerpVal);
            yield return null;
        }
    }

    private IEnumerator ICancelPrecharge() {
        Vector3 targetScale = prechargeWind.transform.localScale;
        targetScale.y = 0;

        while (prechargeWind.transform.localScale.y > 0) {
            prechargeWind.transform.localScale = Vector3.MoveTowards(prechargeWind.transform.localScale, targetScale, Time.deltaTime);
            yield return null;
        }
    }

    private void CancelCharge() {
        chargeDustController.Stop();
        chargeShield.Disable();
    }

    private void LerpPrecharge(float lerpVal) {
        prechargeWind.transform.position = LerpVectorY(prechargeWind.transform.position, prechargeWindYPositionRange, lerpVal);
        prechargeWind.transform.localScale = LerpVectorY(prechargeWind.transform.localScale, prechargeWindYScaleRange, lerpVal);

        ParticleSystem.MainModule main = prechargeWind.main;
        main.startColor = Color.Lerp(prechargeWindStartColor, prechargeWindEndColor, lerpVal);

        ParticleSystem.ColorOverLifetimeModule col = prechargeWind.colorOverLifetime;
        col.color = LerpGradient(prechargeWindStartGradient, prechargeWindEndGradient, lerpVal);

        Gradient ppcol = LerpGradient(prechargeParticlesStartGradient, prechargeParticlesEndGradient, lerpVal);
        prechargeParticles.SetGradient(propertyPrechargeCOL, ppcol);
    }

    private Vector3 LerpVectorY(Vector3 target, Vector2 range, float lerpVal) {
        return new Vector3(target.x, Mathf.Lerp(range.x, range.y, lerpVal), target.z);
    }

    private Gradient LerpGradient(Gradient a, Gradient b, float t) {
        var keysTimes = new List<float>();

        for (int i = 0; i < a.colorKeys.Length; i++) {
            float k = a.colorKeys[i].time;
            if (!keysTimes.Contains(k))
                keysTimes.Add(k);
        }

        for (int i = 0; i < b.colorKeys.Length; i++) {
            float k = b.colorKeys[i].time;
            if (!keysTimes.Contains(k))
                keysTimes.Add(k);
        }

        for (int i = 0; i < a.alphaKeys.Length; i++) {
            float k = a.alphaKeys[i].time;
            if (!keysTimes.Contains(k))
                keysTimes.Add(k);
        }

        for (int i = 0; i < b.alphaKeys.Length; i++) {
            float k = b.alphaKeys[i].time;
            if (!keysTimes.Contains(k))
                keysTimes.Add(k);
        }

        GradientColorKey[] clrs = new GradientColorKey[keysTimes.Count];
        GradientAlphaKey[] alphas = new GradientAlphaKey[keysTimes.Count];

        for (int i = 0; i < keysTimes.Count; i++) {
            float key = keysTimes[i];
            var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
            clrs[i] = new GradientColorKey(clr, key);
            alphas[i] = new GradientAlphaKey(clr.a, key);
        }

        var g = new Gradient();
        g.SetKeys(clrs, alphas);

        return g;
    }
}
