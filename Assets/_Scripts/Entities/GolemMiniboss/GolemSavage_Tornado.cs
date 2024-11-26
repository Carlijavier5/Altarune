using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GolemSavage {
    public partial class GolemSavage {
        private class GolemSavage_Tornado : State<GolemSavageStateInput> {
            // Creating objects
            private GolemSavage golemSavage;
            private Transform player;
            private NavMeshAgent navigation;
            private Damageable damageable;

            private Coroutine startSpin;
            private Coroutine startMove;
            private Coroutine endSpin;
            private Coroutine endMove;
            
            // Base tornado checks and components
            private bool finishTornado = false;
            private bool deactivateTornado = false;
            private float elapsedTime = 0f;
            [SerializeField] private float totalTornadoTime = 15f;

            // Creating spin components (maxSpinSpeed in degrees/second)
            private float currentSpinSpeed = 0f;
            [SerializeField] private float maxSpinSpeed = 2440f;
            [SerializeField] private float spinAccelerationTime = 5f;
            [SerializeField] private float spinDecelerationTime = 4f;

            // Movement components
            private float currentMoveSpeed = 0f;
            [SerializeField] private float maxMoveSpeed = 30f;
            [SerializeField] private float moveAccelerationTime = 4f;
            [SerializeField] private float moveDecelerationTime = 4f;
            [SerializeField] private float rayCastDistance = 1f;
            private Vector3 moveDirection;

            private int phase;

            public GolemSavage_Tornado(int phase) {
                this.phase = phase;
            }

            public override void Enter(GolemSavageStateInput input) {
                // Initializes the enemy using the StateInput
                golemSavage = input.GolemSavage;

                // Initializes the player and damageable
                player = golemSavage.player;
                damageable = golemSavage.damageable;

                // Initilalizes and clears the navmeshagent
                navigation = golemSavage.navigation;
                navigation.SetDestination(golemSavage.transform.position);
                navigation.velocity = Vector3.zero;

                // Pushable implementation
                golemSavage.MotionDriver.Set(navigation);

                // Makes the miniboss invulnerable during the tornado attack
                damageable.ToggleIFrame(true);

                // Initializes a Coroutine used for random spinning
                startSpin = golemSavage.StartCoroutine(StartSpin());
            }

            public override void Update(GolemSavageStateInput input) {
                // Check whether to switch states every frame
                CheckStateTransition();
            }

            // Switches to the next state depending on whether timer is up
            private void CheckStateTransition() {
                if (phase == 1) {
                    if (finishTornado) {
                        damageable.ToggleIFrame(false);
                        golemSavage.stateMachine.SetState(new GolemSavage_PhaseOne());
                    }
                } else if (phase == 3) {
                    if (finishTornado) {
                        damageable.ToggleIFrame(false);
                        golemSavage.stateMachine.SetState(golemSavage.phaseThreeState);
                    }
                }

                
            }

            // Method that starts the spin logic
            IEnumerator StartSpin() {
                float spinElapsedTime = 0f;

                // Acceleration occurs over the spinAccelerationTime value
                while (spinElapsedTime < spinAccelerationTime) {
                    spinElapsedTime += Time.deltaTime;

                    // Creates the smooth acceleration effect (linear interpolation)
                    currentSpinSpeed = Mathf.Lerp(0, maxSpinSpeed, spinElapsedTime / spinAccelerationTime);
                    golemSavage.transform.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime);
                    yield return null;
                }

                // Calls the method to start movement
                startMove = golemSavage.StartCoroutine(StartMove());

                // Continues spinning at top speed
                while (!deactivateTornado) {
                    golemSavage.transform.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime);
                    yield return null;

                    // Determines when to switch to deactivate the torando attack
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime >= totalTornadoTime) {
                        deactivateTornado = true;
                        break;
                    }
                }

                // Calls the spin deceleration method
                endSpin = golemSavage.StartCoroutine(StopSpin());
            }

            // Method that starts the move logic (called after spin is at top speed)
            IEnumerator StartMove() {
                // Initially chooses a direction to start moving in
                ChooseStartDirection();
                float moveElapsedTime = 0f;

                // Acceleration occurs over the moveAccelerationTime value
                while (!deactivateTornado) {
                    if (moveElapsedTime < moveAccelerationTime) {
                        moveElapsedTime += Time.deltaTime;

                        // Creates the smooth acceleration effect (linear interpolation)
                        currentMoveSpeed = Mathf.Lerp(0, maxMoveSpeed, moveElapsedTime / moveAccelerationTime);
                    }

                    // Stores information about the collision (specifically the normal vector)
                    RaycastHit hit;

                    // Casts a Raycast to determine when to change directions (1f away from a wall)
                    if (Physics.Raycast(golemSavage.transform.position, moveDirection, out hit, rayCastDistance)) {
                        // Perfectly reflects the direction off of the wall
                        moveDirection = Vector3.Reflect(moveDirection, hit.normal);

                        // Adds some variation to the reflection
                        float randomVariation = Random.Range(-45f, 45f);
                        moveDirection = Quaternion.Euler(0, randomVariation, 0) * moveDirection;
                    }

                    // Sets the direction and speed in the NavMeshAgent
                    navigation.Move(moveDirection * currentMoveSpeed * Time.deltaTime);

                    yield return null;
                }

                // Calls the move deceleration method
                endMove = golemSavage.StartCoroutine(StopMove());
            }

            // Randomly calculates a vector to move towards
            private void ChooseStartDirection() {
                float randomAngle = Random.Range(0f, 360f);

                // Uses the random angle to rotate a direction vector, creating a random trajectory
                moveDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
            }

            // Method that decelerates spinning to 0
            IEnumerator StopSpin() {
                float spinElapsedTime = 0f;

                // Decelerates the spin over the spinDecelerationTime value
                while (spinElapsedTime < spinDecelerationTime) {
                    spinElapsedTime += Time.deltaTime;
                    currentSpinSpeed = Mathf.Lerp(maxSpinSpeed, 0, spinElapsedTime / spinDecelerationTime);
                    golemSavage.transform.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime);
                    yield return null;
                }
            }

            // Method that decelerates movement to 0
            IEnumerator StopMove() {
                float moveElapsedTime = 0f;

                // Decelerates the movement over the moveDecelerationTime value
                while (moveElapsedTime < moveDecelerationTime) {
                    moveElapsedTime += Time.deltaTime;
                    currentMoveSpeed = Mathf.Lerp(maxMoveSpeed, 0, moveElapsedTime / moveDecelerationTime);
                    navigation.Move(moveDirection * currentMoveSpeed * Time.deltaTime);

                    // Continues to switch directions while decelerating
                    if (Physics.Raycast(golemSavage.transform.position, moveDirection, rayCastDistance)) {
                        ChooseStartDirection();
                    }

                    yield return null;
                }

                // Ends the tornado attack
                finishTornado = true;
            }

            // Method called when exiting the state
            public override void Exit(GolemSavageStateInput input) {
                // Stop the Coroutines
                if (startSpin != null) golemSavage.StopCoroutine(startSpin);
                if (startMove != null) golemSavage.StopCoroutine(startMove);
                if (endSpin != null) golemSavage.StopCoroutine(endSpin);
                if (endMove != null) golemSavage.StopCoroutine(endMove);

                // Makes the enemy vulnerable to damage again
                damageable.ToggleIFrame(false);
            }
        }
    }
}