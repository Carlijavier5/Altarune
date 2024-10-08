using System;
using UnityEngine;

public class GolemSlither_SweepHitbox : MonoBehaviour
{
    [SerializeField] private float sweepDuration = 0.3f;
    [SerializeField] private float sweepSpeed = 15f;

    private float elapsedTime;
    private Vector3 targetScale;
    [NonSerialized] public float timeScale = 1;

    private void Awake() {
        targetScale = transform.localScale;
        transform.localScale = new Vector3(0, targetScale.y, targetScale.z);
    }

    private void Update() {
        float newXScale = Mathf.MoveTowards(transform.localScale.x, targetScale.x, sweepSpeed * Time.deltaTime * timeScale);
        transform.localScale = new Vector3(newXScale, targetScale.y, targetScale.z);

        elapsedTime += Time.deltaTime * timeScale;

        if (elapsedTime >= sweepDuration) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity) && entity.Faction != EntityFaction.Hostile) {
            entity.TryDamage(4);
        }
    }
}
