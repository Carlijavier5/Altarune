using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// TryDamage
namespace Miniboss {
    public partial class Miniboss {
        private class PhaseOne : State<MinibossStateInput> {
            // Creating objects
            private Miniboss miniboss;
            private Transform player;
            private NavMeshAgent navigation;
            private Coroutine malfunctionCoroutine;

            // Creating variables
            private float speed;
            private float stoppingDistance;
            private float health;
            
            // Creating spin components
            private bool spinState = false;
            private readonly float spinDuration = 1;

            // Entrance method
            public override void Enter(MinibossStateInput input) {
                // Initializes the enemy using the StateInput
                miniboss = input.Miniboss;

                // Initializes methods with values from the enemy
                player = miniboss.player;
                navigation = miniboss.navigation;
                health = miniboss.health;

                // Initializes variables with values from the enemy
                speed = miniboss.speed;
                stoppingDistance = miniboss.stoppingDistance;

                // Initializes a Coroutine used for random spinning
                malfunctionCoroutine = miniboss.StartCoroutine(MalfunctionCoroutine());
            }

            // Runs every frame
            public override void Update(MinibossStateInput input) {
                // Switches between rotating the enemy and following the player
                if (spinState == true && navigation.remainingDistance != 0 && !navigation.pathPending) {
                    miniboss.transform.Rotate(Vector3.up, 500f * Time.deltaTime);
                } else {
                    FollowPlayer();
                }
                CheckStateTransition();
            }

            // Switches to the next state depending on the health
            private void CheckStateTransition() {
                if (health <= 50) {
                    miniboss.stateMachine.SetState(new PhaseTwo());
                }
            }

            // Method that outlines how the enemy follows the player
            public void FollowPlayer() {
                // Calculates the normalized vector in the direction of the player
                Vector3 directionToPlayer = (player.position - miniboss.transform.position).normalized;
                // Calculates the angle between the enemy and the player
                float angleToPlayer = Vector3.Angle(miniboss.transform.forward, directionToPlayer);

                // If the angle is less then 20 degrees
                if (angleToPlayer <= 20.0f) {
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

            // Method that randomly (5 - 10 seconds) calls the SpinCoroutine method
            IEnumerator MalfunctionCoroutine() {
                while(true) {
                    yield return new WaitForSeconds(Random.Range(5, 10));
                    yield return SpinCoroutine();
                }
            }

            // Method that performs the spin logic
            IEnumerator SpinCoroutine() {
                spinState = true;
                // Spin duration is 1 second
                yield return new WaitForSeconds(spinDuration);
                // Forces the enemy to look at the player before moving
                LookTowardsPlayer();
                spinState = false;
            }

            public void LookTowardsPlayer() {
                // Determines how the enemy should rotate towards the player
                Vector3 directionToPlayer = (player.position - miniboss.transform.position).normalized;
                Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);
                // Rotates the enemy towards the player (uses slerp)
                miniboss.transform.rotation = Quaternion.Slerp(miniboss.transform.rotation, rotateToPlayer, 2f * Time.deltaTime);
            }

            public override void Exit(MinibossStateInput input) {
                // Stop the Coroutine
                if (malfunctionCoroutine != null) {
                    miniboss.StopCoroutine(malfunctionCoroutine);
                    malfunctionCoroutine = null;
                }
            }
        }
    }
}