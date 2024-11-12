using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GolemSavage {
    public partial class GolemSavage {
        private class GolemSavage_MeteorStrike : State<GolemSavageStateInput> {
            // Creating objects
            private GolemSavage golemSavage;
            private Transform player;
            private NavMeshAgent navigation;
            private Damageable damageable;
            
            // Creating normal variables
            private float health;
            private float speed;
            private float stoppingDistance;

            // Creating the meteor prefab and array
            private GameObject meteorPrefab;
            private GameObject[] meteors;

            // Meteor Strike components
            [SerializeField] int numMeteor = 10;
            [SerializeField] private float hoverSpeed = 0.8f;
            [SerializeField] private float hoverTime = 1f;
            [SerializeField] private float hoverHeight = 4f;
            [SerializeField] private float fallSpeed = 10f;
            bool canMove = false;

            private Coroutine hover;
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
                speed = 5f;
                stoppingDistance = golemSavage.stoppingDistance;

                // Initializes the meteor prefab
                meteorPrefab = golemSavage.meteorPrefab;

                // Makes the enemy invulnerable
                damageable.ToggleIFrame(true);

                SpawnMeteors();
            }

            public void SpawnMeteors() {
                // Possible location bounds for spawning
                float minX = -10f;
                float maxX = 10f;
                float minZ = -10f;
                float maxZ = 10f;
                float positionY = -1f;

                Vector3[] spawnVectors = new Vector3[numMeteor];
                for (int i = 0; i < numMeteor; i++) {
                    // Randomly generate the spawn location vectors
                    float positionX = Random.Range(minX, maxX);
                    float positionZ = Random.Range(minZ, maxZ);
                    spawnVectors[i] = new Vector3(positionX, positionY, positionZ);

                    // Instantiates the meteors below the ground
                    GameObject meteor = Instantiate(meteorPrefab, spawnVectors[i], Quaternion.identity);

                    // Adds the on destruction listener
                    Meteor meteorController = meteor.GetComponent<Meteor>();
                    meteorController.onMeteorDestruction.AddListener(MeteorDestroyed);
                }

                hover = golemSavage.StartCoroutine(HoverCoroutine());
            }

            // Method that makes the miniboss hover before starting the meteor coroutine
            public IEnumerator HoverCoroutine() {
                navigation.enabled = false;
                while (golemSavage.transform.position.y < hoverHeight) {
                    golemSavage.transform.position += Vector3.up * hoverSpeed * Time.deltaTime;
                    yield return null;
                }

                yield return new WaitForSeconds(hoverTime);
                fall = golemSavage.StartCoroutine(FallCoroutine());
            }

            public IEnumerator FallCoroutine() {
                while (golemSavage.transform.position.y > 0f) {
                    golemSavage.transform.position -= Vector3.up * fallSpeed * Time.deltaTime;
                    yield return null;
                }
                
                canMove = true;
                damageable.ToggleIFrame(false);
            }

            public override void Update(GolemSavageStateInput input) {
                health = golemSavage.health;

                if (canMove) {
                    FollowPlayer();
                }
                
                LookTowardsPlayer();
                CheckStateTransition();
            }

            // Method that outlines how the enemy follows the player
            public void FollowPlayer() {
                navigation.enabled = true;

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

            // Switches to the next state depending on the health
            private void CheckStateTransition() {
                if (health == 0 || numMeteor == 0) {
                    golemSavage.stateMachine.SetState(new GolemSavage_PhaseThree());
                }
            }

            public void MeteorDestroyed() {
                numMeteor -= 1;
            }

            public void LookTowardsPlayer() {
                // Determines how the enemy should rotate towards the player
                Vector3 directionToPlayer = (player.position - golemSavage.transform.position).normalized;
                Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);
                // Rotates the enemy towards the player (uses slerp)
                golemSavage.transform.rotation = Quaternion.Slerp(golemSavage.transform.rotation, rotateToPlayer, 2f * golemSavage.DeltaTime);
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

            }
        }
    }
}