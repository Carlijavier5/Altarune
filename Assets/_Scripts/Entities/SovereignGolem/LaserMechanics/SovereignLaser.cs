using System.Collections.Generic;
using UnityEngine;

public abstract class SovereignLaser : MonoBehaviour {

    [Header("Core")]
    [SerializeField] private int damageAmount;
    [SerializeField] private float hitCooldown;

    [Header("Audio")]
    [SerializeField] private SFXOneShot sfxLaserTrigger;
    [SerializeField] private SFXLoop sfxLaserLoop;

    private readonly Stack<BaseObject> terminateStack = new();
    private readonly Dictionary<BaseObject, float> contactMap = new();
    private readonly HashSet<BaseObject> contactSet = new();

    protected virtual void Update() {
        try {
            foreach (KeyValuePair<BaseObject, float> kvp in contactMap) {
                contactMap[kvp.Key] -= Time.deltaTime;
                if (kvp.Value <= 0) terminateStack.Push(kvp.Key);
            }
        } catch { }

        while (terminateStack.TryPop(out BaseObject baseObject)) {
            if (contactSet.Contains(baseObject)) {
                baseObject.TryDamage(damageAmount);
                contactMap[baseObject] = hitCooldown;
            } else contactMap.Remove(baseObject);
        }
    }

    protected void ClearContacts() {
        terminateStack.Clear();
        contactMap.Clear();
        contactSet.Clear();
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)) {
            contactSet.Add(baseObject);
            if (!contactMap.ContainsKey(baseObject)) {
                baseObject.TryDamage(damageAmount);
                contactMap[baseObject] = hitCooldown;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            contactSet.Remove(entity);
        }
    }

    protected void ToggleAudio(bool on) {
        if (on) {
            sfxLaserTrigger.Play();
            sfxLaserLoop.Play();
        } else {
            sfxLaserLoop.Stop();
        }
    }
}