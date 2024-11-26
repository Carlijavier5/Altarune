using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;
using GolemSavage;

namespace GolemSavage {
    public partial class GolemSavage {
        private class GolemSavage_PhaseThree : State<GolemSavageStateInput> {
            // Creating objects
            private GolemSavage golemSavage;
            private Transform player;
            private NavMeshAgent navigation;
            private Damageable damageable;

            // Creating essential variables
            private float health;
            private float speed;
            private float stoppingDistance;

            // Attack selection
            private bool startMeteorStrike = false;
            private bool startMinionSpawn = false;
            private Coroutine selectAttack;
            private int attackSelector;
            private Coroutine spawnMinions;

            public override void Enter(GolemSavageStateInput input) {
                // Initializes the enemy using the StateInput
                golemSavage = input.GolemSavage;

                // Initializes objects with values from the enemy
                player = golemSavage.player;
                navigation = golemSavage.navigation;
                damageable = golemSavage.damageable;

                // Pushable implementation
                golemSavage.MotionDriver.Set(navigation);

                // Initializes variables with values from the enemy
                speed = golemSavage.speed * 2;
                stoppingDistance = 1.5f;
                health = golemSavage.health;

                if (startMeteorStrike == false) {
                    golemSavage.stateMachine.SetState(new GolemSavage_MeteorStrike());
                    startMeteorStrike = true;
                }

                selectAttack = golemSavage.StartCoroutine(SelectAttackCoroutine());
            }

            public override void Update(GolemSavageStateInput input) {
                health = golemSavage.health;
                FollowPlayer();

                if (health == 25) {
                    startMinionSpawn = true;
                    spawnMinions = golemSavage.StartCoroutine(SpawnMinions());
                }
            }

            private IEnumerator SpawnMinions() {
                navigation.isStopped = true;

                Vector3[] minionSpawnLocations = {
                    new Vector3(3f, 0f, 0f),
                    new Vector3(-3f, 0f, 0f),
                    new Vector3(0f, 0f, 0f) 
                };

                GameObject fireMinionPrefab = golemSavage.fireMinionPrefab;
                GameObject waterMinionPrefab = golemSavage.waterMinionPrefab;
                GameObject windMinionPrefab = golemSavage.windMinionPrefab;

                GameObject fireMinion = Object.Instantiate(fireMinionPrefab, minionSpawnLocations[0], Quaternion.identity);
                GameObject waterMinion = Object.Instantiate(waterMinionPrefab, minionSpawnLocations[1], Quaternion.identity);
                GameObject windMinion = Object.Instantiate(windMinionPrefab, minionSpawnLocations[2], Quaternion.identity);

                fireMinion.SetActive(false);
                waterMinion.SetActive(false);
                windMinion.SetActive(false);

                FireMinion fireMinionController = fireMinion.GetComponent<FireMinion>();
                WaterMinion waterMinionController = waterMinion.GetComponent<WaterMinion>();
                WindMinion windMinionController = windMinion.GetComponent<WindMinion>();

                // Waits for 1/2 a second between spawns
                yield return new WaitForSeconds(0.5f);

                fireMinion.SetActive(true);
                fireMinionController.SetPlayer(player);
                yield return new WaitForSeconds(1f);

                waterMinion.SetActive(true);
                waterMinionController.SetPlayer(player);
                yield return new WaitForSeconds(1f);

                windMinion.SetActive(true);
                yield return new WaitForSeconds(1f);

                // Activates every minion
                fireMinionController.Activate();
                waterMinionController.Activate();
                windMinionController.Activate();

                navigation.isStopped = false;
                startMinionSpawn = false;
            }

            // Method that outlines how the enemy follows the player
            public void FollowPlayer() {
                // Calculates the normalized vector in the direction of the player
                Vector3 directionToPlayer = (player.position - golemSavage.transform.position).normalized;
                // Calculates the angle between the enemy and the player
                float angleToPlayer = Vector3.Angle(golemSavage.transform.forward, directionToPlayer);

                // If the angle is less then 60 degrees
                if (angleToPlayer <= 60.0f) {
                    navigation.isStopped = false;
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
                    navigation.isStopped = true;
                    LookTowardsPlayer();
                }
            }

            public void LookTowardsPlayer() {
                // Determines how the enemy should rotate towards the player
                Vector3 directionToPlayer = (player.position - golemSavage.transform.position).normalized;
                Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);
                // Rotates the enemy towards the player (uses slerp)
                golemSavage.transform.rotation = Quaternion.Slerp(golemSavage.transform.rotation, rotateToPlayer, 2f * Time.deltaTime);
            }

            // Method that randomly (5 - 10 seconds) chooses an attack
            public IEnumerator SelectAttackCoroutine() {
                yield return new WaitForSeconds(10);
                while(true) {
                    yield return new WaitForSeconds(Random.Range(5, 10));

                    // Selects an attack (1, 2)
                    attackSelector = Random.Range(1, 3);

                    if (attackSelector == 1) {
                        golemSavage.stateMachine.SetState(new GolemSavage_Tornado(3));
                    } else if (attackSelector == 2) {
                        golemSavage.stateMachine.SetState(new GolemSavage_GroundSlam(30, 100, 3));
                    }
                }
            }

            // Method to try to damage non-hostile factions
            public void OnTriggerEnter(Collider other) {
                if (other.TryGetComponent(out Entity entity)
                    && entity.Faction != EntityFaction.Hostile) {
                    bool isDamageable = entity.TryDamage(3);
                }
            }

            public override void Exit(GolemSavageStateInput input) {
                // Stop the Coroutine
                if (selectAttack != null) golemSavage.StopCoroutine(selectAttack);
                if (spawnMinions != null) golemSavage.StopCoroutine(spawnMinions);
            }
        }
    }
}