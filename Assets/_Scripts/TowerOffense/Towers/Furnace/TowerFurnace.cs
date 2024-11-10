using System.Collections;
using UnityEngine;

public class TowerFurnace : Summon {

    [SerializeField] private TowerProjectile projectilePrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float attackInterval;
    [SerializeField] private float angleShift;

    private float angle;
    private float attackTick = 0.2f;

    void Awake() {
        angle = Random.Range(0, 360);
    }

    protected override void Update() {
        if (!active) return;
        base.Update();
        attackTick += Time.deltaTime;
        if (attackTick >= attackInterval) {
            TowerProjectile projectile = Instantiate(projectilePrefab, launchPoint.transform.position, Quaternion.identity);
            Quaternion myRotation = Quaternion.AngleAxis(angle = (angle + angleShift) % 360, Vector3.up);
            Vector3 startingDirection = transform.right;
            Vector3 result = myRotation * startingDirection;
            projectile.Launch(result);
            attackTick = 0;
        }
    }
}