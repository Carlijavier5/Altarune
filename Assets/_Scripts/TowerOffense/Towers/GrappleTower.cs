using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleTower : Summon
{
    //[SerializeField] private TowerProjectile grappleSegment;
    //[SerializeField] private Transform launchPoint;
    [SerializeField] private float range;
    [SerializeField] private float minRange;
    [SerializeField] private float grappleSpeed;
    [SerializeField] private float grappleTime;
    [SerializeField] private float cooldownInterval;

    private SphereCollider sCollider;

    private float angle;
    private float grappleTick = 0.0f;

    void Awake() {
        angle = UnityEngine.Random.Range(0, 360);
        sCollider = GetComponent<SphereCollider>();
        sCollider.radius = range;
    }

    protected override void Update() {
        base.Update();
        //Debug.Log(sCollider.radius);
        if (!active) return;
        grappleTick += Time.deltaTime;
        if (grappleTick >= cooldownInterval) {
            //Find all nearby gameobjects
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, range);
            GameObject[] objectsInRange = new GameObject[collidersInRange.Length];
            for (int i = 0; i < collidersInRange.Length; i++) {
                objectsInRange[i] = collidersInRange[i].gameObject;
            }

            //find the nearest gameobject with a golem component (note: this should be broadened to include all monsters/enemies that are movable)
            float greatestDist = 0;
            GolemSentinel furthestGolem = null;
            foreach (GameObject go in objectsInRange) {
                if (go.TryGetComponent<GolemSentinel>(out furthestGolem)) {
                    if (furthestGolem == null || getDistance(transform.position, go.transform.position) > greatestDist) {
                        greatestDist = getDistance(transform.position, go.transform.position);
                    }
                }
            }
            if (furthestGolem != null) {

                grappleTick = 0;
            }
            // TowerProjectile projectile = Instantiate(projectilePrefab, launchPoint.transform.position, Quaternion.identity);
            // Quaternion myRotation = Quaternion.AngleAxis(angle = (angle + angleShift) % 360, Vector3.up);
            // Vector3 startingDirection = transform.right;
            // Vector3 result = myRotation * startingDirection;
            // projectile.Launch(result);
            
        }
    }

    private float getDistance(Vector3 pos1, Vector3 pos2) {
        float dx = pos1.x - pos2.x;
        float dy = pos1.y - pos2.y;
        float dz = pos1.z - pos2.z;
        return (float) Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
}
