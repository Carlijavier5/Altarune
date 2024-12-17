

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

public class EnemyWaveSpawner : Entity {
    [SerializeField] public List<PseudoList> waves;
    [SerializeField] public int maxEnemiesAtOnce = 5; // Max enemies from this tower at a time
    [SerializeField] public float spawnDelay = 2.0f; // Delay between enemy spawns
    // [SerializeField] public Transform spawnPos;
    [SerializeField] public float spawnDistance;
    [SerializeField] private AggroRange aggroRange;
    
    private List<Entity> enemies;
    private IEnumerable<Rigidbody> rigidbodies;
    private int enemyIndex;
    private float _nextSpawnTime;
    private bool isActive;
    private int currentWave;


    public void Awake() {
        enemies = new List<Entity>(maxEnemiesAtOnce);
        isActive = false;
        rigidbodies = GetComponentsInChildren<Rigidbody>(true).Where((rb) => rb.gameObject != gameObject);

        aggroRange.OnAggroEnter += e => {Activate();};
        aggroRange.OnAggroExit += e => {Deactivate();};
        currentWave = 0;
        enemyIndex = 0;
    }

    protected override void Update() {
        base.Update();
        if (isActive && Time.time > _nextSpawnTime) {
            _nextSpawnTime = Time.time + spawnDelay;

            if (enemies.Count < maxEnemiesAtOnce && enemyIndex < waves[currentWave].Count()) {
            

                Entity enemyPrefab = (Entity) waves[currentWave][enemyIndex];
                enemyIndex++;

                Vector2 vec = Random.insideUnitCircle.normalized * spawnDistance;
                Vector3 newPos = transform.position + new Vector3(vec[0], 0, vec[1]);
                Bounds bounds = enemyPrefab.GetComponent<Collider>().bounds;
                float longestBound = System.Math.Max(bounds.extents.x, bounds.extents.z);

                // Check for entity within the chosen spawn location
                // ensures no enemy stacking, or spawning right on top of player
                Collider[] colls = Physics.OverlapSphere(newPos, longestBound + 1.5f);
                foreach (Collider col in colls) {
                    Entity entity = col.gameObject.GetComponent<Entity>();
                    // ignore spawn collisions with the spawner itself; use spawnradius instead.
                    if (!col.isTrigger && entity != null && entity.GetType() != typeof(EntitySpawner)) {
                        // if failed to place enemy, don't reset timer
                        _nextSpawnTime = Time.time + 0.1f;
                        return;
                    }
                }

                Entity newEnemy = Instantiate(enemyPrefab, newPos, Quaternion.identity);
                // remove enemies from this object's list on perish so they don't get deleted alongside the spawner
                newEnemy.OnPerish += e=>{
                    enemies?.Remove(newEnemy);
                    if (enemies?.Count <= 0) {
                        currentWave++;
                        enemyIndex = 0;
                    }
                };

                enemies.Add(newEnemy);
                if (waves[currentWave].innerList.Count() <= enemyIndex && currentWave >= waves.Count - 1) {
                    // if it's the last wave and we've run out of enemies, perish
                    Perish(false);
                }
            } 
        }
    }

    public override void Perish(bool immediate) {
        base.Perish(immediate);
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
        Destroy(this);
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