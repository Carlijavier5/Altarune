using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSniper : Summon {

    [SerializeField] private TowerProjectile projectilePrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float attackInterval;
    
    private bool init;

    //private float angle;
    private float attackTick = 0.2f;

    // Add comparator to sort by lowest health entities
    // SortedSet<Entity> targets = new SortedSet<Entity>(
    //     Comparer<Entity>.Create((a, b) => a.GetComponent<Damageable>().Health.CompareTo(b.GetComponent<Damageable>().Health)));
    List<Entity> targets = new List<Entity>();
    protected override void Awake() {
        base.Awake();
        //angle = 0;
    }

    public override void Init(Player player) {
        base.Init(player);
        init = true;
    }

    protected override void Update() {
        if (!init) return;
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
            }
            attackTick = 0;
        }
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