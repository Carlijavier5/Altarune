using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SovereignSlam : MonoBehaviour {

    public event System.Action OnSmashEnd;

    [SerializeField] private float maxSize;
    [SerializeField] private AnimationCurve growthCurve;

    

    public void DoSlam(float duration) {
        
    }

    private IEnumerator IDoSlam(float duration) {
        float lerpVal, sizeMult, timer = 0;
        while (timer < duration) {
            timer = Mathf.MoveTowards(timer, duration, Time.deltaTime);
            lerpVal = timer / duration;
            sizeMult = growthCurve.Evaluate(lerpVal);
            transform.localScale = new Vector3(1, 0, 1) * sizeMult;
            yield return null;
        }
    }

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
                //entity.TryDamage(damageAmount);
                //contactMap[entity] = hitCooldown;
            } else contactMap.Remove(entity);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            contactSet.Add(entity);
            if (!contactMap.ContainsKey(entity)) {
                //entity.TryDamage(damageAmount);
                //contactMap[entity] = hitCooldown;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            contactSet.Remove(entity);
        }
    }
}