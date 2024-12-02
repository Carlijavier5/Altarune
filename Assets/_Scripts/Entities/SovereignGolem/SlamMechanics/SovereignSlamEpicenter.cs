using System.Collections.Generic;
using UnityEngine;

public class SovereignSlamEpicenter : MonoBehaviour {

    public event System.Action<BaseObject> OnObjectEnter;

    public readonly HashSet<BaseObject> contactSet = new();

    public void Clear() => contactSet.Clear();

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)) {
            contactSet.Add(baseObject);
            OnObjectEnter?.Invoke(baseObject);
        }
    }
}