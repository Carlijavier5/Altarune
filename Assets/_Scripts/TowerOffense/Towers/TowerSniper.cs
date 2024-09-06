using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSniper : Summon {

    [SerializeField] private TowerProjectile projectilePrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float attackInterval;
    [SerializeField] private float angleShift;
    
    private bool init;

    private float angle;
    private float attackTick = 0.2f;

    // Add comparator to sort by lowest health entities
    SortedSet<Entity> targets = new SortedSet<Entity>(
        Comparer<Entity>.Create((a, b) => a.GetHealth().CompareTo(b.GetHealth)));

    protected override void Awake() {
        base.Awake();
        angle = 0;
    }

    public override void Init() => init = true;

    void Update() {
        if (!init) return;
        attackTick += Time.deltaTime;
        if (attackTick >= attackInterval) {
            // Make projectile
            TowerProjectile projectile = Instantiate(projectilePrefab, launchPoint.transform.position, Quaternion.identity);
            // Aim Projectile
            Quaternion myRotation = Quaternion.AngleAxis(angle = (angle + angleShift) % 360, Vector3.up);
            Vector3 startingDirection = transform.right;
            Vector3 result = myRotation * startingDirection;
            // Fire Projectile
            projectile.Launch(result);
            attackTick = 0;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            // Entity health = priority, low health = higher priority
            if (entity.GetType() != typeof(PlayerController)) {
                targets.Add(entity);
                Debug.Log("got a new target in the list");
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            targets.Remove(entity);
        }
    }

    Entity GetTarget() {
        return targets.Min;
    }
}