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
    private bool init;
    private SphereCollider sCollider;

    private float angle;
    private float grappleTick = 0.0f;

    protected override void Awake() {
        base.Awake();
        angle = UnityEngine.Random.Range(0, 360);
        sCollider = GetComponent<SphereCollider>();
        sCollider.radius = range;
    }

    public override void Init() => init = true;

    void Update() {
        //Debug.Log(sCollider.radius);
        if (!init) return;
        grappleTick += Time.deltaTime;
        if (grappleTick >= cooldownInterval) {
            // TowerProjectile projectile = Instantiate(projectilePrefab, launchPoint.transform.position, Quaternion.identity);
            // Quaternion myRotation = Quaternion.AngleAxis(angle = (angle + angleShift) % 360, Vector3.up);
            // Vector3 startingDirection = transform.right;
            // Vector3 result = myRotation * startingDirection;
            // projectile.Launch(result);
            grappleTick = 0;
        }
    }
}
