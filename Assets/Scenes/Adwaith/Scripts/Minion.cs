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

        // Creating health variable
        private float health;

        // Creating default values for movement
        [SerializeField] private float stoppingDistance = 1.5f;
        private float speed;
        
        // Creating minion death components
        private bool setActive = false;
        public UnityEvent onMinionDeath;

        void Start() {
            // Initializes the base objects
            navigation = GetComponent<UnityEngine.AI.NavMeshAgent>();
            damageable = GetComponent<Damageable>();
            damageable.ToggleIFrame(false);

            // Initializes health
            health = damageable.Health;
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

        void Update() {
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

        // Method that tells listeners a game object is gone
        public void HandleDeath() {
            onMinionDeath.Invoke();
            gameObject.SetActive(false);
        }
    }
}