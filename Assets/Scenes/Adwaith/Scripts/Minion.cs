using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Miniboss {
    public class Minion : Entity {
        // Creating the NavMeshAgent, Player, and Damageable
        private UnityEngine.AI.NavMeshAgent navigation;
        private Transform player;
        private Damageable damageable;

        // Creating default values for movement
        [SerializeField] private float stoppingDistance = 1.5f;
        public float RootMult => CanMove ? 1 : 0;
        private float speed;

        // Creating health variable
        private float health;
        
        // Creating minion death components
        private bool setActive = false;
        public UnityEvent onMinionDeath;

        public void Start() {
            // Initializes the base objects
            navigation = GetComponent<UnityEngine.AI.NavMeshAgent>();
            damageable = GetComponent<Damageable>();
            damageable.ToggleIFrame(false);

            // Pushable implementation
            MotionDriver.Set(navigation);

            // Initializes health
            health = damageable.Health;

            // Adding listeners
            OnStunSet += setStunned;
            OnRootSet += setRooted;
            OnTimeScaleSet += setTimeScaled;
        }

        // Sets the Player transform
        public void SetPlayer(Transform player) {
            this.player = player;
        }

        // Activates the minion object, making it visible and setting the speed
        public void Activate() {
            speed = Random.Range(2f, 5f);
            navigation.speed = speed;
            gameObject.SetActive(true);
            setActive = true;
        }

        protected override void Update() {
            if (player != null && setActive == true) {
                FollowPlayer();
            }
            // Updates health
            health = damageable.Health;
        }

        // Method that handles following the player
        public void FollowPlayer() {
            navigation.stoppingDistance = stoppingDistance;
            navigation.autoBraking = true;
            navigation.SetDestination(player.position);
        }

        // Method to try to damage non-hostile factions
        public void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out Entity entity)
                && entity.Faction != EntityFaction.Hostile) {
                bool isDamageable = entity.TryDamage(3);
            }
        }

        // Method called when the enemy is stunned
        private void setStunned(bool isStunned) {
            if (isStunned) {
                navigation.speed = 0;
            } else {
                navigation.speed = speed;
            }
        }

        // Method called when the enemy is rooted
        private void setRooted(bool canMove) {
            navigation.speed = speed * status.timeScale * RootMult;
        }

        // Method called when the time scale is adjusted
        private void setTimeScaled(float timeScale) {
            navigation.speed = speed * timeScale * RootMult;
        }

        // Method called when enemy loses all its health
        public override void Perish() {
            onMinionDeath.Invoke();
            DetachModules();
            enabled = false;
            Destroy(gameObject, 5);
            base.Perish();
        }
    }
}