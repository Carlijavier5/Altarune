using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelShield : MonoBehaviour {

    [SerializeField] private Collider attackCollider;
    [SerializeField] private int damageAmount;

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)
                && !baseObject.IsFaction(EntityFaction.Hostile)) {
            baseObject.TryDamage(damageAmount);
        }
    }

    public void Enable() => attackCollider.enabled = true;
    public void Disable() => attackCollider.enabled = false;
}