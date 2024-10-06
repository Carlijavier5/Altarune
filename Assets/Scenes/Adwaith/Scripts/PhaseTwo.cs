using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;

namespace Miniboss {
    public partial class Miniboss {
        private class PhaseTwo : State<MinibossStateInput> {
            // Creating necessary variables
            private Miniboss miniboss;
            private Transform player;
            private NavMeshAgent navigation;

            private float health;
            private float speed;
            private float stoppingDistance;

            private bool finishedDuplication = false;

            private int numMinions;


            // Initializing the minion prefab and array
            private GameObject minionPrefab;
            private GameObject[] minions;

            public override void Enter(MinibossStateInput input) {
                miniboss = input.Miniboss;

                // Initializes methods with values from the enemy
                player = miniboss.player;
                navigation = miniboss.navigation;
                minionPrefab = miniboss.minionPrefab;

                // Initializes variables with values from the enemy
                speed = miniboss.speed / 2;
                stoppingDistance = miniboss.stoppingDistance;

                if (minionPrefab == null) {
                    Debug.Log(minionPrefab);
                }

                // Spawns the minions
                InitializeMinions();

                // Duplicates the minions
                miniboss.StartCoroutine(DuplicateMinion(miniboss.transform.position));
            }

            public override void Update(MinibossStateInput input) {
                // Initializes variables that will change during this lifecycle
                health = miniboss.health;

                if (finishedDuplication) {
                    LookTowardsPlayer();
                    CheckStateTransition();
                }
            }

            private void CheckStateTransition() {
                if (health <= 5) {
                    miniboss.stateMachine.SetState(new PhaseThree());
                }
            }

            // Method to initialize the minions
            private void InitializeMinions() {
                // Spawns between 4 and 6 minions
                int numMinions = Random.Range(4, 6);
                minions = new GameObject[numMinions];

                // Minions are spawned at (0, 0, 0), but hidden
                for (int i = 0; i < numMinions; i++) {
                    minions[i] = Object.Instantiate(minionPrefab, Vector3.zero, Quaternion.identity);
                    minions[i].SetActive(false);
                }
            }

            public void LookTowardsPlayer() {
                // Determines how the enemy should rotate towards the player
                Vector3 directionToPlayer = (player.position - miniboss.transform.position).normalized;

                Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);
                // Rotates the enemy towards the player (uses slerp)
                miniboss.transform.rotation = Quaternion.Slerp(miniboss.transform.rotation, rotateToPlayer, 2f * Time.deltaTime);
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
                float minDistanceFromEnemy = 1f;

                foreach (GameObject minion in minions) {
                    // Makes the minions visible
                    minion.SetActive(true);
                    Vector3 spawnOffset;
                    
                    // Finds a good location for the minions to spawn
                    do {
                        float offsetX = Random.Range(-2.5f, 2.5f);
                        float offsetZ = Random.Range(-2.5f, 2.5f);
                        spawnOffset = new Vector3(offsetX, 0, offsetZ);
                    } while (spawnPositions.Any(pos => 
                        Vector3.Distance(enemyPos + spawnOffset, pos) < minDistance));

                    // Sets the minion spawn location and updates the list
                    minion.transform.position = enemyPos + spawnOffset;
                    spawnPositions.Add(minion.transform.position);
                    activateMinions.Add(minion);

                    // Sets the player in the minion controller (can't set via GUI for prefabs)
                    MinionController minionController = minion.GetComponent<MinionController>();
                    minionController.SetPlayer(player);
                    minionController.onMinionDeath.AddListener(HandleMinionDeath);

                    // Waits for 1/2 a second between spawns
                    yield return new WaitForSeconds(1f);
                }
                
                numMinions = activateMinions.Count;

                // Activates every minion
                foreach(GameObject minion in activateMinions) {
                    minion.GetComponent<MinionController>().Activate();
                }

                // Allows the enemy to begin to move towards the player again
                finishedDuplication = true;
            }

            public void HandleMinionDeath() {
                health -= 25 / (numMinions);
            }

            public override void Exit(MinibossStateInput input) {
                
            }
        }
    }
}