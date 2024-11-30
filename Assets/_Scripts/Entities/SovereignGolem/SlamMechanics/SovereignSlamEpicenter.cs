using System.Collections.Generic;
using UnityEngine;

public class SovereignSlamEpicenter : MonoBehaviour {

    public event System.Action<Entity> OnEntityEnter;

    public readonly HashSet<Entity> contactSet = new();

    public void Clear() => contactSet.Clear();

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            contactSet.Add(entity);
            OnEntityEnter?.Invoke(entity);
        }
    }
}