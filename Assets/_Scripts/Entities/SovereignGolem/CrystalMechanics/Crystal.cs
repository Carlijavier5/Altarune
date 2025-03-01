using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Crystal : Entity {

    private const string HOLO_THRESHOLD_KEY = "_Threshold_Parameter?";

    [SerializeField] private DefaultSummonProperties settings;
    [SerializeField] private ManaShield manaShield;
    [SerializeField] private Material introMaterial;
    [SerializeField] private VisualEffect introFX;
    [SerializeField] private float introDuration;

    private float timer;

    void Start() {
        StartCoroutine(IFormCrystal());
        TryToggleIFrame(true);
        manaShield.OnPerish += ManaShield_OnPerish;
        manaShield.Activate();
    }

    private IEnumerator IFormCrystal() {
        introFX.Play();
        ApplyMaterial(introMaterial);
        Transform t = objectBody;

        float lerpVal;
        while (timer < introDuration) {
            timer = Mathf.MoveTowards(timer, introDuration, Time.deltaTime);
            lerpVal = timer / introDuration;
            t.localScale = new Vector3(settings.growthCurveXZ.Evaluate(lerpVal),
                                       settings.growthCurveY.Evaluate(lerpVal),
                                       settings.growthCurveXZ.Evaluate(lerpVal));
            UpdatePropertyBlock((mpb) => { mpb.SetFloat(HOLO_THRESHOLD_KEY, 1 - lerpVal); });
            yield return null;
        }

        RemoveMaterial(introMaterial);
    }

    private IEnumerator IShatterCrystal() {
        introFX.Play();
        ApplyMaterial(introMaterial);
        Transform t = objectBody;

        float lerpVal;
        while (timer > 0) {
            timer = Mathf.MoveTowards(timer, 0, Time.unscaledDeltaTime);
            lerpVal = timer / introDuration;
            UpdatePropertyBlock((mpb) => { mpb.SetFloat(HOLO_THRESHOLD_KEY, 1 - lerpVal); });
            t.localScale = new Vector3(settings.growthCurveXZ.Evaluate(lerpVal),
                                       settings.growthCurveY.Evaluate(lerpVal),
                                       settings.growthCurveXZ.Evaluate(lerpVal));
            yield return null;
        }

        Destroy(gameObject, 0.2f);
    }

    private void ManaShield_OnPerish(BaseObject _) {
        TryToggleIFrame(false);
    }

    public override void Perish(bool immediate) {
        base.Perish(immediate);
        StartCoroutine(IShatterCrystal());
    }
}