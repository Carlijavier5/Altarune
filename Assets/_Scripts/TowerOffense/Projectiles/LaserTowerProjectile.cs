using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LaserTowerProjectile : MonoBehaviour {

    //Change
	[SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
	[SerializeField] private int damage = 1;
    private Vector3 ogScale;

	void OnCollisionEnter(Collision coll) {
        if ((coll.collider is MeshCollider
            || coll.collider is BoxCollider
            || coll.collider is TerrainCollider)) {
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
        while (transform.localScale != Vector3.zero) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * 5);
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