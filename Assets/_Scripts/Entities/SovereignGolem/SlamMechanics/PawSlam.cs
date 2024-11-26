using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawSlam : MonoBehaviour {

    public event System.Action<PawSlam> OnSmashEnd;

    [SerializeField] private int damageAmount;
    [SerializeField] private float hitCooldown, maxSize;
    [SerializeField] private SovereignSlamEpicenter outerEpicenter,
                                                    innerEpicenter;
    [SerializeField] private AnimationCurve growthCurve;

    private readonly Stack<Entity> terminateStack = new();
    private readonly Dictionary<Entity, float> contactMap = new();

    private void Awake() {
        outerEpicenter.OnEntityEnter += OuterEpicenter_OnEntityEnter;
        innerEpicenter.OnEntityExit += InnerEpicenter_OnEntityExit;
    }

    void Update() {
        try {
            foreach (KeyValuePair<Entity, float> kvp in contactMap) {
                contactMap[kvp.Key] -= Time.deltaTime;
                if (kvp.Value <= 0) terminateStack.Push(kvp.Key);
            }
        } catch { }

        while (terminateStack.TryPop(out Entity entity)) {
            if (outerEpicenter.contactSet.Contains(entity)
                && !innerEpicenter.contactSet.Contains(entity)) {
                entity.TryDamage(damageAmount);
                contactMap.Remove(entity);
            } else contactMap.Remove(entity);
        }
    }

    public void DoSlam(Vector3 source, float duration) {
        transform.position = source;
        gameObject.SetActive(true);
        StartCoroutine(IDoSlam(duration));
    }

    private IEnumerator IDoSlam(float duration) {
        yield return new WaitForEndOfFrame();

        float lerpVal, scaleVal, timer = 0;
        while (timer < duration) {
            timer = Mathf.MoveTowards(timer, duration, Time.deltaTime);
            lerpVal = timer / duration;
            scaleVal = growthCurve.Evaluate(lerpVal) * maxSize;
            transform.localScale = Vector3.one * scaleVal;
            yield return null;
        }
        OnSmashEnd?.Invoke(this);
        gameObject.SetActive(false);
    }

    private void OuterEpicenter_OnEntityEnter(Entity entity) {
        if (!contactMap.ContainsKey(entity)
            && !innerEpicenter.contactSet.Contains(entity)) {
            entity.TryDamage(damageAmount);
            contactMap[entity] = hitCooldown;
        }
    }

    private void InnerEpicenter_OnEntityExit(Entity entity) {
        if (!contactMap.ContainsKey(entity)
            && outerEpicenter.contactSet.Contains(entity)) {
            entity.TryDamage(damageAmount);
            contactMap[entity] = hitCooldown;
        }
    }
}