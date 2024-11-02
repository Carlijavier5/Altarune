

// enemyspawner is a stationary tower, normally idle
// once it aggros it spawns a set of enemies and then waits until they're all dead
// then it spawns the next wave
// the spawner script doesnt need a state machine, if we want a tower to aggro, we can define that behaviour somewhere else

// you should be able to define what enemies it spawns, how many waves it spawns, 
// and possibly the enemy makeup of each wave
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : Entity {
    [SerializeField] public Entity enemyPrefab;
    [SerializeField] public int totalEnemyCount = 10; // Total enemies this tower can spawn
    [SerializeField] public int maxEnemiesAtOnce = 5; // Max enemies from this tower at a time
    [SerializeField] public float spawnDelay = 2.0f; // Delay between enemy spawns
    // [SerializeField] public Transform spawnPos;
    [SerializeField] public float spawnDistance;
    [SerializeField] private AggroRange aggroRange;
    
    private List<Entity> enemies;
    private IEnumerable<Rigidbody> rigidbodies;
    private int enemiesLeft;
    private float _nextSpawnTime;
    private bool isActive;
    public void Awake() {
        enemies = new List<Entity>(maxEnemiesAtOnce);
        enemiesLeft = totalEnemyCount;
        isActive = false;
        rigidbodies = GetComponentsInChildren<Rigidbody>(true).Where((rb) => rb.gameObject != gameObject);

        aggroRange.OnAggroEnter += e => {Activate();};
        aggroRange.OnAggroExit += e => {Deactivate();};
    }

    protected override void Update() {
        base.Update();
        if (isActive && Time.time > _nextSpawnTime) {
            _nextSpawnTime = Time.time + spawnDelay;
            // spawn an enemy
            if (enemies.Count < maxEnemiesAtOnce) {
                // probably redo this?
                Vector2 vec = Random.insideUnitCircle.normalized * spawnDistance;
                Vector3 newPos = transform.position + new Vector3(vec[0], vec[1], 0);

                Entity newEnemy = Instantiate(enemyPrefab, newPos, Quaternion.identity);
                newEnemy.OnPerish += e=>{enemies?.Remove(newEnemy);};

                enemies.Add(newEnemy);
                enemiesLeft--;
                if (enemiesLeft <= 0) {
                    Perish();
                }
            } 
        }
    }

    public override void Perish() {
        base.Perish();
        foreach (Rigidbody rb in rigidbodies) {
            rb.isKinematic = false;
            Vector3 force = new Vector3(Random.Range(-0.15f, 0.15f), 0.85f, Random.Range(-0.15f, 0.15f)) * Random.Range(250, 300);
            rb.AddForce(force);
            Vector3 torque = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)) * Random.Range(250, 300);
            rb.AddTorque(torque);
        }
        foreach (Entity e in enemies) {
            e.transform.SetParent(null, true);
        }
        enemies = null;
        DetachModules();
        Destroy(gameObject, 2);
        enabled = false;
    }

    public void Activate() {
        isActive = true;
        _nextSpawnTime = Time.time + spawnDelay;
    }
    public void Deactivate() {
        isActive = false;
    }
}