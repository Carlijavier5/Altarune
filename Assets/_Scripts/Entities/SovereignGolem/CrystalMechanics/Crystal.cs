using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Crystal : Entity {

    private const string HOLO_THRESHOLD_KEY = "_Threshold_Parameter?";
    //private const string STATE_NAME = "TP-Intro";

    //[SerializeField] private Animator animator;
    [SerializeField] private ManaShield manaShield;
    [SerializeField] private Material introMaterial;
    [SerializeField] private VisualEffect introFX;
    [SerializeField] private float introDuration;

    //private int lerpParam;
    private float timer;

    void Start() {
        //lerpParam = Animator.StringToHash(HOLO_THRESHOLD_KEY);
        //animator.Play(STATE_NAME);
        StartCoroutine(IFormCrystal());
        TryToggleIFrame(true);
        manaShield.OnPerish += ManaShield_OnPerish;
        manaShield.Activate();
    }

    private IEnumerator IFormCrystal() {
        introFX.Play();
        ApplyMaterial(introMaterial);

        float lerpVal;
        while (timer < introDuration) {
            timer = Mathf.MoveTowards(timer, introDuration, Time.deltaTime);
            lerpVal = timer / introDuration;
            UpdatePropertyBlock((mpb) => { mpb.SetFloat(HOLO_THRESHOLD_KEY, 1 - lerpVal); });
            yield return null;
        }

        RemoveMaterial(introMaterial);
    }

    private IEnumerator IShatterCrystal() {
        introFX.Play();
        ApplyMaterial(introMaterial);

        float lerpVal;
        while (timer > 0) {
            timer = Mathf.MoveTowards(timer, 0, Time.unscaledDeltaTime);
            lerpVal = timer / introDuration;
            UpdatePropertyBlock((mpb) => { mpb.SetFloat(HOLO_THRESHOLD_KEY, 1 - lerpVal); });
            yield return null;
        }

        Destroy(gameObject, 0.2f);
    }

    private void ManaShield_OnPerish(BaseObject _) {
        TryToggleIFrame(false);
    }

    public override void Perish() {
        base.Perish();
        StartCoroutine(IShatterCrystal());
    }
}