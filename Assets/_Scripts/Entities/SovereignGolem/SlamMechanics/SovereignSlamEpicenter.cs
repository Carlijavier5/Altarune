using System.Collections.Generic;
using UnityEngine;

public class SovereignSlamEpicenter : MonoBehaviour {

    public event System.Action<Entity> OnEntityEnter;
    public event System.Action<Entity> OnEntityExit;

    public readonly HashSet<Entity> contactSet = new();

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            contactSet.Add(entity);
            OnEntityEnter?.Invoke(entity);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            contactSet.Remove(entity);
            OnEntityExit?.Invoke(entity);
        }
    }
}