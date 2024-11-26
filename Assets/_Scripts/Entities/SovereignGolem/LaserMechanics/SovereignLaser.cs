using System.Collections.Generic;
using UnityEngine;

public abstract class SovereignLaser : MonoBehaviour {

    [SerializeField] private int damageAmount;
    [SerializeField] private float hitCooldown;
    private readonly Stack<Entity> terminateStack = new();
    private readonly Dictionary<Entity, float> contactMap = new();
    private readonly HashSet<Entity> contactSet = new();

    protected virtual void Update() {
        try {
            foreach (KeyValuePair<Entity, float> kvp in contactMap) {
                contactMap[kvp.Key] -= Time.deltaTime;
                if (kvp.Value <= 0) terminateStack.Push(kvp.Key);
            }
        } catch { }

        while (terminateStack.TryPop(out Entity entity)) {
            if (contactSet.Contains(entity)) {
                entity.TryDamage(damageAmount);
                contactMap[entity] = hitCooldown;
            } else contactMap.Remove(entity);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            contactSet.Add(entity);
            if (!contactMap.ContainsKey(entity)) {
                entity.TryDamage(damageAmount);
                contactMap[entity] = hitCooldown;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            contactSet.Remove(entity);
        }
    }
}