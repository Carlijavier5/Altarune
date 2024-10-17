using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LaserTowerProjectile : MonoBehaviour {

    //Change
	[SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
	[SerializeField] private int damage = 1;
    [SerializeField] private float animTimeScale = 15f;
    [SerializeField] private float lifetime = 0.25f;
    private Vector3 ogScale = new Vector3(1f, 1f, 1f);
    private float lifetimeTracker;

    void Awake() {
        ogScale.x = transform.localScale.x;
        ogScale.y = transform.localScale.y;
        transform.localScale = Vector3.zero;
        lifetimeTracker = 0f;
    }

    //Strictly required to set the length of the laser.
    public void setOGscale(float stretch) {
        ogScale.z = stretch;
        Debug.Log(stretch);
        StartCoroutine(IGrow());
    }

	void OnCollisionEnter(Collision coll) {
        if ((coll.collider is MeshCollider
            || coll.collider is BoxCollider
            || coll.collider is TerrainCollider)) {
        }
    }

    private IEnumerator IGrow() {
        while (lifetimeTracker < lifetime) {
            if (transform.localScale.z != ogScale.z)
                transform.localScale = Vector3.MoveTowards(transform.localScale, ogScale, Time.deltaTime * animTimeScale);
            lifetimeTracker += Time.deltaTime;
            if (lifetimeTracker >= lifetime) {
                End();
            }
            yield return null;
        }
    }

    private IEnumerator IEnd() {
        while (transform.localScale != Vector3.zero) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * animTimeScale);
            yield return null;
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            entity.TryDamage(damage);
            End();
        }
    }

    private void End() {
        Destroy(rb);
        Destroy(coll);
        StartCoroutine(IEnd());
    }

}