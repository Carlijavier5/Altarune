using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorEffectorAoE : MonoBehaviour {

    [SerializeField] private Collider attackCollider;
    [SerializeField] private int damageAmount;
    [SerializeField] private float lingerDuration;

    private readonly HashSet<BaseObject> contactSet = new();

    public void DoDamage() {
        contactSet.Clear();
        attackCollider.enabled = true;
        StopAllCoroutines();
        StartCoroutine(IDoDuration());
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)
            && !baseObject.IsFaction(EntityFaction.Hostile)
                && !contactSet.Contains(baseObject)) {
            contactSet.Add(baseObject);
            baseObject.TryDamage(damageAmount);
        }
    }

    private IEnumerator IDoDuration() {
        yield return new WaitForSeconds(lingerDuration);
        attackCollider.enabled = false;
    }
}