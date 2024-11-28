using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlitherSweepHitbox : MonoBehaviour {

    public event System.Action OnAnticipationEnd;

    [SerializeField] private Collider attackCollider;

    private readonly HashSet<Entity> contactSet = new();
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
        foreach (Entity entity in contactSet) {
            entity.TryDamage(damageAmount);
        }
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
        yield return null;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)
            && !entity.IsFaction(EntityFaction.Hostile)) {
            contactSet.Add(entity);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Entity entity)
            && !entity.IsFaction(EntityFaction.Hostile)) {
            contactSet.Remove(entity);
        }
    }
}