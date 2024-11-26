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

            private MeteorSpawner meteorSpawner;

            // Meteor Strike components
            [SerializeField] private float hoverSpeed = 0.8f;
            [SerializeField] private float hoverTime = 2f;
            [SerializeField] private float hoverHeight = 4f;
            [SerializeField] private float fallSpeed = 10f;
            bool finishState = false;

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

                // Makes the enemy invulnerable
                damageable.ToggleIFrame(true);

                meteorSpawner = golemSavage.GetComponent<MeteorSpawner>();
                meteorSpawner.StartMeteorSpawning();
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
                
                yield return new WaitForSeconds(0.2f);
                damageable.ToggleIFrame(false);
                finishState = true;
            }

            public void LookTowardsPlayer() {
                // Determines how the enemy should rotate towards the player
                Vector3 directionToPlayer = (player.position - golemSavage.transform.position).normalized;
                Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);
                // Rotates the enemy towards the player (uses slerp)
                golemSavage.transform.rotation = Quaternion.Slerp(golemSavage.transform.rotation, rotateToPlayer, 2f * Time.deltaTime);
            }

            public override void Update(GolemSavageStateInput input) {
                health = golemSavage.health;
                LookTowardsPlayer();

                if (finishState) {
                    navigation.enabled = true;
                    golemSavage.stateMachine.SetState(golemSavage.phaseThreeState);
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
                // Stop the Coroutines
                if (hover != null) golemSavage.StopCoroutine(hover);
                if (fall != null) golemSavage.StopCoroutine(fall);
            }
        }
    }
}