using System.Collections;
using UnityEngine;

public class TowerProjectile : MonoBehaviour {

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float lifetime;

    private bool active;
    private Vector3 dir;
    private Vector3 ogScale;

    void Awake() {
        ogScale = transform.localScale;
        transform.localScale = Vector3.zero;
        StartCoroutine(IGrow());
    }

    public void Launch(Vector3 direction) {
        dir = direction.normalized;
        active = true;
    }

    void Update() {
        if (active) rb.MovePosition(rb.position + dir * speed);
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) End();
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)
            && !(baseObject is Entity
                 && (baseObject as Entity).Faction == EntityFaction.Friendly)) {
            if (baseObject.TryDamage(4)) End();
        } else if (!other.isTrigger && other.gameObject.layer != 4) {
            End();
        }
    }

    private IEnumerator IGrow() {
        while (transform.localScale != ogScale) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, ogScale, Time.deltaTime * 5);
            yield return null;
        }
    }

    private IEnumerator IEnd() {
        while (transform.localScale.magnitude > Mathf.Epsilon) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * 5);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void End() {
        active = false;
        coll.enabled = false;

        StopAllCoroutines();
        StartCoroutine(IEnd());
    }
}