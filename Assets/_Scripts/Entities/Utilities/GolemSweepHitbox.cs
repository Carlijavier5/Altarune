using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSweepHitbox : MonoBehaviour {

    public event System.Action OnAnticipationEnd;

    [SerializeField] private TwoColoredGraphicFader graphicFader;
    [SerializeField] private Collider attackCollider;

    public bool Active => attackCollider.enabled;

    private readonly HashSet<BaseObject> contactSet = new();

    private float xScaleTarget;

    private void Awake() {
        xScaleTarget = transform.localScale.x;
        transform.localScale = new Vector3(0, transform.localScale.y,
                                           transform.localScale.z);
    }

    public void DoAnticipation(Entity caster, float duration) {
        attackCollider.enabled = true;
        StopAllCoroutines();
        StartCoroutine(IDoAnticipation(caster, duration));
        graphicFader.DoFade(true);
    }

    public void CancelSweep() {
        StopAllCoroutines();
        graphicFader.DoFade(false);
        attackCollider.enabled = false;
        contactSet.Clear();
    }

    public void DoDamage(int damageAmount) {
        attackCollider.enabled = false;
        foreach (BaseObject baseObject in contactSet) {
            baseObject.TryDamage(damageAmount);
        }
        graphicFader.DoFade(false);
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