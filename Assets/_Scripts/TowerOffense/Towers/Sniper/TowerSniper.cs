using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerSniper : Summon {

    [SerializeField] private TowerProjectile projectilePrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float attackInterval;
    [SerializeField] private Transform barrel;
    [SerializeField] private float rotateSpeed = 0.5f;

    //private float angle;
    private float attackTick = 0.2f;
    private SniperProjectileController _controller;

    // Add comparator to sort by lowest health entities
    // SortedSet<Entity> targets = new SortedSet<Entity>(
    //     Comparer<Entity>.Create((a, b) => a.GetComponent<Damageable>().Health.CompareTo(b.GetComponent<Damageable>().Health)));
    List<Entity> targets = new List<Entity>();

    private void Start() {
        _controller = GetComponentInChildren<SniperProjectileController>();
    }

    protected override void Update() {
        if (!active) return;
        base.Update();
        attackTick += Time.deltaTime;
        if (attackTick >= attackInterval) {
            Entity lowestHealthEnemy = GetTarget();
            if (lowestHealthEnemy != null) {
                // Make projectile
                TowerProjectile projectile = Instantiate(projectilePrefab, launchPoint.transform.position, Quaternion.identity);

                // Aim Projectile
                Vector3 direction = lowestHealthEnemy.transform.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Vector3 result = targetRotation * Vector3.forward;

                // Fire Projectile
                projectile.Launch(result);
                _controller.Fire();
            }
            attackTick = 0;
        }
        
        Entity target = GetTarget();
        Vector3 targetDir = target.transform.position - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(targetDir);
        Vector3 orient = targetRot.eulerAngles;
        orient.x = 0;
        orient.z = 0;
        _controller.transform.position = target.transform.position;
        //Rotate
        barrel.rotation = LerpTowardsRotation(barrel.rotation, Quaternion.Euler(orient), Time.deltaTime * rotateSpeed);
    }
    
    Quaternion LerpTowardsRotation(Quaternion current, Quaternion target, float lerpFactor)
    {
        // Interpolates between the current and target rotation
        return Quaternion.Lerp(current, target, lerpFactor);
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            // Entity health = priority, low health = higher priority
            if (entity.GetType() != typeof(PlayerController)) {
                if (entity != null) {
                    targets.Add(entity);
                }
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            targets.Remove(entity);
        }
    }

    Entity GetTarget() {
        targets.RemoveAll(enemy => enemy == null || enemy.Faction == EntityFaction.Friendly);
        targets.Sort(Comparer<Entity>.Create((a, b) => a.Health.CompareTo(b.Health)));
        if (targets.Count > 0) return targets[0];
        else return null;
    }
}