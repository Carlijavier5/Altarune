using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GolemSavage {
    public class WindMinion : Entity {
        // Creating the NavMeshAgent, Player, and Damageable
        private UnityEngine.AI.NavMeshAgent navigation;
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
        public UnityEvent windMinionEvent;

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

        // Activates the minion object, making it visible and setting the speed
        public void Activate() {
            speed = 10f;
            navigation.speed = speed;
            gameObject.SetActive(true);
            setActive = true;
        }

        protected override void Update() {
            // Updates health
            health = damageable.Health;
            MoveRandomly();
        }

        public void ChooseDirection() {
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            randomDirection.Normalize();

            Vector3 destination = transform.position + randomDirection * 4f;

            navigation.SetDestination(destination);
        }

        public void MoveRandomly() {
            ChooseDirection();
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position, navigation.destination - transform.position, out hit, 4f)) {
            ChooseDirection();
            }
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