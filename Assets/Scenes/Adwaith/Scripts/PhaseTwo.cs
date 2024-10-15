using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;
using Miniboss;

namespace Miniboss {
    public partial class Miniboss {
        private class PhaseTwo : State<MinibossStateInput> {
            // Creating objects
            private Miniboss miniboss;
            private Transform player;
            private NavMeshAgent navigation;
            private Damageable damageable;
            private Coroutine moveToCenter;

            // Creating essential variables
            private float health;
            private float speed;
            private float stoppingDistance;

            // Creating minion logic
            private bool finishedDuplication = false;
            private int numMinions;
            private Vector3[] minionSpawnLocations = new Vector3[6];

            // Creating the minion prefab and array
            private GameObject minionPrefab;
            private GameObject[] minions;

            public override void Enter(MinibossStateInput input) {
                // Initializes the enemy using the StateInput
                miniboss = input.Miniboss;

                // Initializes objects with values from the enemy
                player = miniboss.player;
                navigation = miniboss.navigation;
                minionPrefab = miniboss.minionPrefab;
                damageable = miniboss.damageable;

                // Pushable implementation
                miniboss.MotionDriver.Set(navigation);

                // Initializes variables with values from the enemy
                speed = miniboss.speed / 1.5f;
                stoppingDistance = 0;
                health = miniboss.health;

                // Initializes the NavMeshAgent
                navigation.speed = speed;
                navigation.stoppingDistance = stoppingDistance;

                // Makes the enemy invulnerable during this phase
                damageable.ToggleIFrame(true);

                // Initializes the minion spawn locations
                minionSpawnLocations = new Vector3[] {
                    new Vector3(1.5f, 0f, 0f),
                    new Vector3(-1.5f, 0f, 0f),
                    new Vector3(-1.5f, 0f, 1.5f),
                    new Vector3(1.5f, 0f, 1.5f),
                    new Vector3(-1.5f, 0f, -1.5f),
                    new Vector3(1.5f, 0f, -1.5f)
                };

                // Makes the enemy move to the center of the map
                moveToCenter = miniboss.StartCoroutine(MoveToCenter(new Vector3(0, 0, 0)));
            }

            private IEnumerator MoveToCenter(Vector3 targetPos) {
                do {
                    navigation.SetDestination(targetPos);
                    yield return null;
                } while (navigation.remainingDistance != 0);

                // Spawns the minions
                InitializeMinions();

                // Duplicates the minions
                miniboss.StartCoroutine(DuplicateMinion(miniboss.transform.position));
            }

            // Method to initialize the minions
            private void InitializeMinions() {
                // Spawns between 4 and 6 minions
                numMinions = Random.Range(4, 6);
                minions = new GameObject[numMinions];

                // Minions are spawned at (0, 0, 0), but hidden
                for (int i = 0; i < numMinions; i++) {
                    minions[i] = Object.Instantiate(minionPrefab, minionSpawnLocations[i], Quaternion.identity);
                    minions[i].SetActive(false);
                }
            }

            IEnumerator DuplicateMinion(Vector3 enemyPos) {
                // Waits for 1/2 a second between spawns
                yield return new WaitForSeconds(0.5f);

                // Stores all minions.  After they all spawn, they attack
                List<GameObject> activateMinions = new List<GameObject>();

                foreach (GameObject minion in minions) {
                    // Makes the minions visible
                    minion.SetActive(true);                    
                    
                    // Adds the minions to the list
                    activateMinions.Add(minion);

                    // Sets the player in the minion controller (can't set via GUI for prefabs)
                    Minion minionController = minion.GetComponent<Minion>();
                    minionController.SetPlayer(player);

                    // Adds the on death listener
                    minionController.onMinionDeath.AddListener(HandleMinionDeath);
 
                    yield return new WaitForSeconds(1f);
                }

                // Activates every minion
                foreach(GameObject minion in activateMinions) {
                    minion.GetComponent<Minion>().Activate();
                }

                // Allows the enemy to look at the player again
                finishedDuplication = true;
            }

            public override void Update(MinibossStateInput input) {
                CheckStateTransition();
                if (finishedDuplication) {
                    LookTowardsPlayer();
                }
            }

            // Method to try to damage non-hostile factions
            public void OnTriggerEnter(Collider other) {
                if (other.TryGetComponent(out Entity entity)
                    && entity.Faction != EntityFaction.Hostile) {
                    bool isDamageable = entity.TryDamage(3);
                }
            }

            private void CheckStateTransition() {
                if (health <= 40) {
                    miniboss.stateMachine.SetState(new PhaseThree());
                }
            }

            public void LookTowardsPlayer() {
                // Determines how the enemy should rotate towards the player
                Vector3 directionToPlayer = (player.position - miniboss.transform.position).normalized;
                Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);
                // Rotates the enemy towards the player (uses slerp)
                miniboss.transform.rotation = Quaternion.Slerp(miniboss.transform.rotation, rotateToPlayer, 2f * Time.deltaTime);
            }

            public void HandleMinionDeath() {
                numMinions -= 1;
                if (numMinions == 0) {
                    health = 40;
                }
            }

            public override void Exit(MinibossStateInput input) {
                // Makes the enemy damageable again
                damageable.ToggleIFrame(false);

                // Stops the Coroutine
                if (moveToCenter != null) {
                    miniboss.StopCoroutine(moveToCenter);
                    moveToCenter = null;
                }
            }
        }
    }
}