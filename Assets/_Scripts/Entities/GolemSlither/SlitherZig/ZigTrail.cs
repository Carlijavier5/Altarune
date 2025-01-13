using System.Collections.Generic;
using UnityEngine;

public class ZigTrail : GraphicFader {

    [SerializeField] private Collider attackCollider;
    private readonly HashSet<BaseObject> contactSet = new();

    public void DoDamage(int damageAmount) {
        attackCollider.enabled = false;
        foreach (BaseObject baseObject in contactSet) {
            baseObject.TryDamage(damageAmount);
        }
    }

    public void Generate(Vector3 position, Quaternion rotation, float zScale) {
        attackCollider.enabled = true;
        transform.SetPositionAndRotation(position, rotation);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, zScale);
        DoFade(true);
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