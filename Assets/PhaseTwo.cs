using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;

public class PhaseTwo : MonoBehaviour, IEnemyActions {
    // Creating necessary variables
    private EnemyController enemy;
    private Transform player;
    private UnityEngine.AI.NavMeshAgent navigation;

    private float health;

    private float speed;
    private float stoppingDistance;

    private bool runOnce = false;
    private bool finishedDuplication = false;

    private int numMinions;

    public void Enter(EnemyController enemy) {
        // Ensures the method only runs once
        if (runOnce) return;
        runOnce = true;

        // Initializes the enemy with the current EnemyController
        this.enemy = enemy;

        // Initializes variables with values from the enemy
        player = enemy.Player;
        navigation = enemy.Navigation;

        speed = enemy.Speed;
        stoppingDistance = enemy.StoppingDistance;

        // Duplicates the minions
        enemy.StartCoroutine(DuplicateMinion(enemy.transform.position));
    }

    public void Execute() {
        // Initializes variables that will change during this lifecycle
        health = enemy.Health;

        if (finishedDuplication) {
            FollowPlayer();
            speed = speed / 2;
        }
    }

    public void FollowPlayer() {
        // Calculates the normalized vector in the direction of the player
        Vector3 directionToPlayer = (player.position - enemy.transform.position).normalized;
        // Calculates the angle between the enemy and the player
        float angleToPlayer = Vector3.Angle(enemy.transform.forward, directionToPlayer);

        // If the angle is less then 20 degrees
        if (angleToPlayer <= 10.0f) {
            navigation.speed = speed;
            navigation.stoppingDistance = stoppingDistance;
            navigation.autoBraking = true;
            navigation.SetDestination(player.position);

            // Logic to ensure the enemy does not move around if the player moves nearby
            if ((navigation.remainingDistance <= navigation.stoppingDistance) && !navigation.pathPending) {
                navigation.isStopped = true;
            } else {
                navigation.isStopped = false;
            }
        } else {
            LookTowardsPlayer();
        }
    }

    public void LookTowardsPlayer() {
        // Determines how the enemy should rotate towards the player
        Vector3 directionToPlayer = (player.position - enemy.transform.position).normalized;

        Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);
        // Rotates the enemy towards the player (uses slerp)
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, rotateToPlayer, 2f * Time.deltaTime);
    }

    IEnumerator DuplicateMinion(Vector3 enemyPos) {
        yield return new WaitForSeconds(0.5f);

        // Stores previous spawn locations
        List<Vector3> spawnPositions = new List<Vector3>();
        spawnPositions.Add(enemyPos);

        // Stores all minions.  After they all spawn, they attack
        List<GameObject> activateMinions = new List<GameObject>();

        // Minimum distance minions should be from each other
        float minDistance = 1.8f;

        foreach (GameObject minion in enemy.Minions) {
            // Makes the minions visible
            minion.SetActive(true);
            Vector3 spawnOffset;
            
            // Finds a good location for the minions to spawn
            do {
                float offsetX = Random.Range(-2.5f, 2.5f);
                float offsetZ = Random.Range(-2.5f, 2.5f);
                spawnOffset = new Vector3(offsetX, 0, offsetZ);
            } while (spawnPositions.Any(pos => Vector3.Distance(enemyPos + spawnOffset, pos) < minDistance));

            // Sets the minion spawn location and updates the list
            minion.transform.position = enemyPos + spawnOffset;
            spawnPositions.Add(minion.transform.position);
            activateMinions.Add(minion);

            // Sets the player in the minion controller (can't set via GUI for prefabs)
            /*MinionController minionController = minion.GetComponent<MinionController>();
            minionController.SetPlayer(player);
            minionController.onMinionDeath.AddListener(HandleMinionDeath);*/

            // Waits for 1/2 a second between spawns
            yield return new WaitForSeconds(1f);
        }
        
        numMinions = activateMinions.Count;

        // Activates every minion
        foreach(GameObject minion in activateMinions) {
            //minion.GetComponent<MinionController>().Activate();
        }

        // Allows the enemy to begin to move towards the player again
        finishedDuplication = true;
    }

    public void HandleMinionDeath() {
        health -= 25 / (numMinions);
    }

    public void Exit() { }
}
