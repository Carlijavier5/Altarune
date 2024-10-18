using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporalArea : MonoBehaviour {

    public event System.Action<Collider> OnContactStay;
    public event System.Action<Collider> OnContactExit;

    [SerializeField] private SphereCollider coll;
    [SerializeField] private float applicationFrequency;

    private readonly Dictionary<Collider, float> timerMap = new();

    public void SetRadius(float radius) => coll.radius = radius;

    void OnTriggerStay(Collider other) {
        if (timerMap.ContainsKey(other)) {
            timerMap[other] -= Time.deltaTime;
            if (timerMap[other] < 0) {
                timerMap[other] = applicationFrequency;
                OnContactStay?.Invoke(other);
            }
        } else timerMap[other] = 0;
    }

    void OnTriggerExit(Collider other) {
        timerMap.Remove(other);
        OnContactExit?.Invoke(other);
    }
}