using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class AltLaserTowerBeam : MonoBehaviour {

    //Change
	[SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    [SerializeField] private float damage = 1, animTimeScale = 15f, fullRampUpTime = 0.5f;
    private Vector3 ogScale = new Vector3(1f, 1f, 1f);
    private Entity target;
    private float range = 100f, beamContactTime = 0f;
    //Uitlity to let this clear the firing towers' reference to this beam
    private Action clearBeam;

    //This is my hack for working around Unity running Awake() right after this is created & then running IGrow() & this self-destructing immediately b/c nobody has given it essential data.
    public void giveData(float range, Entity target, Action clearBeamFunction) {
        this.range = range;
        this.target = target;
        clearBeam = clearBeamFunction;
        StartCoroutine(IGrow());
    }

    void Awake() {
        ogScale.x = transform.localScale.x;
        ogScale.y = transform.localScale.y;
        transform.localScale = Vector3.zero;
    }

	void OnCollisionEnter(Collision coll) {
        if ((coll.collider is MeshCollider
            || coll.collider is BoxCollider
            || coll.collider is TerrainCollider
            )) {
                coll.gameObject.TryGetComponent<Entity>(out Entity e);
                if (e == null || !e.Equals(target)) {
                    End();
                }
        }
    }

    //Because Unity can't give me an xz() Vector2
    private Vector2 flatten(Vector3 v) {
        return new Vector2(v.x, v.z);
    }

    private IEnumerator IGrow() {
        while (target != null && Vector3.Distance(gameObject.transform.position, target.transform.position) <= range) {
            ogScale.z = Vector2.Distance(flatten(target.gameObject.transform.position), flatten(gameObject.transform.position))/2;
            if (transform.localScale.z != ogScale.z)
                transform.localScale = Vector3.MoveTowards(transform.localScale, ogScale, Time.deltaTime * animTimeScale);
            if (transform.localRotation != Quaternion.LookRotation(target.transform.position - gameObject.transform.position)) {
                transform.localRotation = Quaternion.Euler(0, Quaternion.LookRotation(target.transform.position - gameObject.transform.position).eulerAngles.y, 0);//Quaternion.RotateTowards(transform.localRotation, Quaternion.LookRotation(target.transform.position - gameObject.transform.position), Time.deltaTime * animTimeScale * 1.5f).eulerAngles.y, 0);
            }
            yield return null;
        }
        End();
    }

    private IEnumerator IEnd() {
        while (transform.localScale != Vector3.zero) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * animTimeScale);
            yield return null;
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other) {
        if (coll.gameObject.TryGetComponent<Entity>(out Entity e) && e.Equals(target)) {
            beamContactTime = 0f;
        }
    }

    void OnTriggerStay(Collider other) {
        if (coll.gameObject.TryGetComponent<Entity>(out Entity e) && e.Equals(target)) {
            beamContactTime += Time.deltaTime;
            e.TryDamage((int) (damage * (beamContactTime / fullRampUpTime)));
        }
    }

    private void End() {
        Destroy(rb);
        Destroy(coll);
        clearBeam();
        StartCoroutine(IEnd());
    }

}