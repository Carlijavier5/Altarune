using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GolemSavage {
    public partial class GolemSavage {
        private class GolemSavage_GroundSlam : State<GolemSavageStateInput> {
            // Creating objects
            private GolemSavage golemSavage;
            private Transform player;
            private NavMeshAgent navigation;
            private Damageable damageable;
            
            // Creating normal variables
            private float health;
            private float speed;
            private float stoppingDistance;

            // Ground slam components
            bool finishSlam = false;
            [SerializeField] float riseSpeed = 10f;
            [SerializeField] float riseDeceleration = 9f;
            [SerializeField] float fallAcceleration = 25f;
            [SerializeField] float maxFallSpeed = 100f;

            // Coroutines
            private Coroutine rise;
            private Coroutine fall;

            public override void Enter(GolemSavageStateInput input) {
                // Initializes the enemy using the StateInput
                golemSavage = input.GolemSavage;

                // Initializes objects with values from the enemy
                player = golemSavage.player;
                navigation = golemSavage.navigation;
                damageable = golemSavage.damageable;

                // Initializes variables with values from the enemy
                health = golemSavage.health;

                // Disables navigation
                navigation.enabled = false;

                // Starts the rise Coroutine
                rise = golemSavage.StartCoroutine(RiseCoroutine());
            }

            // Method that makes the miniboss hover before starting the meteor coroutine
            public IEnumerator RiseCoroutine() {
                while (riseSpeed != 0f) {
                    riseSpeed -= riseDeceleration * Time.deltaTime;
                    riseSpeed = Mathf.Max(riseSpeed, 0f);

                    golemSavage.transform.position += Vector3.up * riseSpeed * Time.deltaTime;
                    yield return null;
                }

                yield return new WaitForSeconds(0.1f);
                fall = golemSavage.StartCoroutine(FallCoroutine());
            }
            

            public IEnumerator FallCoroutine() {
                float currentFallSpeed = 0f;

                while (golemSavage.transform.position.y > 0f) {
                    currentFallSpeed += fallAcceleration * Time.deltaTime;
                    currentFallSpeed = Mathf.Min(currentFallSpeed, maxFallSpeed);
                    
                    golemSavage.transform.position -= Vector3.up * currentFallSpeed * Time.deltaTime;
                    yield return null;
                }
                finishSlam = true;
            }

            public override void Update(GolemSavageStateInput input) {
                health = golemSavage.health;
                LookTowardsPlayer();
                CheckStateTransition();
            }

            public void LookTowardsPlayer() {
                // Determines how the enemy should rotate towards the player (ignoring Y)
                float rotateX = player.position.x - golemSavage.transform.position.x;
                float rotateZ = player.position.z - golemSavage.transform.position.z;
                Vector3 directionToPlayer = new Vector3(rotateX, 0f, rotateZ).normalized;
                Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);

                // Keeps the enemy's Y direction the same
                float currentYRotation = golemSavage.transform.rotation.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, rotateToPlayer.eulerAngles.y, 0f);

                // Rotates the enemy towards the player (uses slerp)
                golemSavage.transform.rotation = Quaternion.Slerp(golemSavage.transform.rotation, rotateToPlayer, 2f * golemSavage.DeltaTime);
            }

            // Switches to the next state depending on the health
            private void CheckStateTransition() {
                if (health <= 50 || finishSlam) {
                    golemSavage.stateMachine.SetState(new GolemSavage_PhaseOne());
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
                // Re-enables navigation
                navigation.enabled = true;

                // Disables all coroutines
                golemSavage.StopCoroutine(rise);
                golemSavage.StopCoroutine(fall);
            }
        }
    }
}