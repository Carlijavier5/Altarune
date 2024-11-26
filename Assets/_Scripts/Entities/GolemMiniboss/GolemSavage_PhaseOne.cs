using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GolemSavage {
    public partial class GolemSavage {
        private class GolemSavage_PhaseOne : State<GolemSavageStateInput> {
            // Creating objects
            private GolemSavage golemSavage;
            private Transform player;
            private NavMeshAgent navigation;
            private Damageable damageable;

            // Creating attack components
            private Coroutine selectAttack;
            private int attackSelector;

            // Creating variables
            private float speed;
            private float stoppingDistance;
            private float health;

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
                health = golemSavage.health;
                speed = golemSavage.speed;
                stoppingDistance = golemSavage.stoppingDistance;

                // Initializes Coroutines
                selectAttack = golemSavage.StartCoroutine(SelectAttackCoroutine());
            }

            public override void Update(GolemSavageStateInput input) {
                health = golemSavage.health;
                speed = golemSavage.speed;
                
                FollowPlayer();
                CheckStateTransition();
            }

            // Switches to the next state depending on the health
            private void CheckStateTransition() {
                if (health <= 70) {
                    golemSavage.stateMachine.SetState(new GolemSavage_PhaseTwo());
                }
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
                golemSavage.transform.rotation = Quaternion.Slerp(golemSavage.transform.rotation, rotateToPlayer, 2f * golemSavage.DeltaTime);
            }

            // Method that randomly (5 - 10 seconds) chooses an attack
            public IEnumerator SelectAttackCoroutine() {
                while(true) {
                    yield return new WaitForSeconds(Random.Range(5, 10));

                    // Selects an attack (1, 2)
                    attackSelector = Random.Range(1, 3);

                    if (attackSelector == 1) {
                        golemSavage.stateMachine.SetState(new GolemSavage_Tornado(1));
                    } else if (attackSelector == 2) {
                        golemSavage.stateMachine.SetState(new GolemSavage_GroundSlam(10, 50, 1));
                    }
                }
            }

            public override void Exit(GolemSavageStateInput input) {
                // Stop the Coroutine
                golemSavage.StopCoroutine(selectAttack);
            }
        }
    }
}