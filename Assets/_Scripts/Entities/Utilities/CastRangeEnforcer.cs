using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastRangeEnforcer : MonoBehaviour {

    public event System.Action<Entity> OnContactCancel;

    [SerializeField] private Entity requiredContact;
    [SerializeField] private Collider contactCollider;
    
    public void Toggle(bool on) {
        contactCollider.enabled = on;
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Entity entity)
                && entity == requiredContact) {
            OnContactCancel?.Invoke(requiredContact);
        }
    }
}