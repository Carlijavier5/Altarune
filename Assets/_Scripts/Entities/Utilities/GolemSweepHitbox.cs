using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSweepHitbox : MonoBehaviour {

    private const string COLOR_PARAM = "_Color";

    public event System.Action OnAnticipationEnd;

    [SerializeField] private Collider attackCollider;
    [SerializeField] private Renderer decal;
    [SerializeField] private float fadeTime;
    private float timer;

    public bool Active => attackCollider.enabled;

    private readonly HashSet<BaseObject> contactSet = new();

    private float xScaleTarget;

    private void Awake() {
        xScaleTarget = transform.localScale.x;
        transform.localScale = new Vector3(0, transform.localScale.y,
                                           transform.localScale.z);

        Color color = decal.sharedMaterial.GetColor(COLOR_PARAM);
        color.a = 0;
        MaterialPropertyBlock mpb = new();
        decal.GetPropertyBlock(mpb);
        mpb.SetColor(COLOR_PARAM, color);
        decal.SetPropertyBlock(mpb);
    }

    public void DoAnticipation(Entity caster, float duration) {
        attackCollider.enabled = true;
        StopAllCoroutines();
        StartCoroutine(IDoAnticipation(caster, duration));
        StartCoroutine(IDoFade(true));
    }

    public void CancelSweep() {
        StopAllCoroutines();
        StartCoroutine(IDoFade(false));
        attackCollider.enabled = false;
        contactSet.Clear();
    }

    public void DoDamage(int damageAmount) {
        attackCollider.enabled = false;
        foreach (BaseObject baseObject in contactSet) {
            baseObject.TryDamage(damageAmount);
        } StartCoroutine(IDoFade(false));
        contactSet.Clear();
    }

    private IEnumerator IDoAnticipation(BaseObject baseObject, float duration) {
        float lerpVal, xScale, timer = 0;
        while (timer < duration) {
            timer = Mathf.MoveTowards(timer, duration, baseObject.DeltaTime);
            lerpVal = timer / duration;
            xScale = Mathf.Lerp(0, xScaleTarget, lerpVal);
            transform.localScale = new Vector3(xScale, transform.localScale.y,
                                               transform.localScale.z);
            yield return null;
        }

        OnAnticipationEnd?.Invoke();
    }

    private IEnumerator IDoFade(bool on) {

        float lerpVal, target = on ? fadeTime : 0;
        Color color = decal.sharedMaterial.GetColor(COLOR_PARAM);

        MaterialPropertyBlock mpb = new();
        decal.GetPropertyBlock(mpb);

        while (Mathf.Abs(target - timer) > 0) {
            timer = Mathf.MoveTowards(timer, target, Time.deltaTime);
            lerpVal = timer / fadeTime;
            color.a = Mathf.Lerp(0, 1, lerpVal);
            mpb.SetColor(COLOR_PARAM, color);
            decal.SetPropertyBlock(mpb);
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)
                && !baseObject.IsFaction(EntityFaction.Hostile)) {
            contactSet.Add(baseObject);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)) {
            contactSet.Remove(baseObject);
        }
    }
}