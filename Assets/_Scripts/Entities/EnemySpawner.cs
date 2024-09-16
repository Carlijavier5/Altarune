

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
    [SerializeField] public Transform spawnPos;
    [SerializeField] private AggroRange aggroRange;
    
    private List<Entity> enemies;
    private int enemiesLeft;
    private float _nextSpawnTime;
    private bool isActive;
    public void Awake() {
        enemies = new List<Entity>(maxEnemiesAtOnce);
        enemiesLeft = totalEnemyCount;
        isActive = false;

        aggroRange.OnAggroEnter += e => {Activate();};
        aggroRange.OnAggroExit += e => {Deactivate();};

        OnPerish += e=>{enabled = false;Debug.LogError("Disabled!!");};

    }

    protected override void Update() {
        base.Update();
        
        if (isActive && Time.time > _nextSpawnTime) {
            Debug.Log(enabled);

            _nextSpawnTime = Time.time + spawnDelay;

            // spawn an enemy
            if (enemies.Count < maxEnemiesAtOnce) {
                // TODO: reuse old enemies
                Entity newEnemy = Instantiate(enemyPrefab, spawnPos);
                newEnemy.OnPerish += e=>{enemies.Remove(newEnemy);};

                enemies.Add(newEnemy);
                enemiesLeft--;
                if (enemiesLeft <= 0) {
                    Perish();
                }
            } 
        }
    }

    public void Activate() {
        isActive = true;
        _nextSpawnTime = Time.time + spawnDelay;
    }
    public void Deactivate() {
        isActive = false;
    }
}