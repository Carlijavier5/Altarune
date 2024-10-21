using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Miniboss {
    public partial class Miniboss : Entity {
        // Creating the NavMeshAgent, Player, Damageable, and the minionPrefab
        private NavMeshAgent navigation;
        private Transform player;
        private Damageable damageable;
        [SerializeField] private GameObject minionPrefab;

        // Initializing default values for movement
        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float stoppingDistance = 1.5f;
        [SerializeField] private Rigidbody rb;
        public float RootMult => CanMove ? 1 : 0;
        
        // Creating health variable
        private float health;

        // Creating the state machine
        private StateMachine<MinibossStateInput> stateMachine;

        // Creating the enemy phases
        private PhaseOne phaseOneState;
        private PhaseTwo phaseTwoState;
        private PhaseThree phaseThreeState;
        private Stunned stunnedPhase;

        private bool active;

        public void Start() {
            // Initializes base components
            navigation = GetComponent<NavMeshAgent>();
            damageable = GetComponent<Damageable>();
        }

        void OnEnable() => TryStart();

        private void TryStart() {
            if (FindPlayer()) {
                active = true;

                // Initializes health
                health = damageable.Health;

                // Initializing variables with Phase files
                phaseOneState = new PhaseOne();
                phaseTwoState = new PhaseTwo();
                phaseThreeState = new PhaseThree();
                stunnedPhase = new Stunned();

                // Initializing the state machine and setting initial phase
                stateMachine = new StateMachine<MinibossStateInput>();
                MinibossStateInput stateInput = new MinibossStateInput(this);
                stateMachine.Init(stateInput, phaseOneState);

                // Adding listeners
                OnStunSet += setStunned;
                OnRootSet += setRooted;
                OnTimeScaleSet += setTimeScaled;
            }
        }

        // Finds the player, and assigns it to the Transform player
        public bool FindPlayer() {
            // Initializes the OverlapSphere collider
            Collider[] findPlayerCollider = Physics.OverlapSphere(
                transform.position, 
                10f, 
                LayerMask.GetMask("Player")
            );

            bool foundPlayer = findPlayerCollider != null && findPlayerCollider.Length > 0;

            // If the player collider is found, assigns it to the transform
            if (foundPlayer) {
                player = findPlayerCollider[0].transform;
            } else {
                StartCoroutine(AwaitFindPlayer());
            }
            return foundPlayer;
        }

        private IEnumerator AwaitFindPlayer() {
            yield return new WaitForSeconds(1);
            TryStart();
        }

        protected override void Update() {
            base.Update();
            // Updates the health
            if (active) {
                health = damageable.Health;
                stateMachine.Update();
            }
        }

        // Method called when the enemy is stunned
        private void setStunned(bool isStunned) {
            // Only stuns if the enemy is in Phase 1 or 3
            if (isStunned && (health > 70 || health <= 40)) {
                stateMachine.SetState(new Stunned());
            } else {
                // Logic to switch back to current state
                if (health > 70) {
                    stateMachine.SetState(new PhaseOne());
                }
                stateMachine.SetState(new PhaseThree());
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

        // Method to try to damage non-hostile factions
        void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out Entity entity)
                && entity.Faction != EntityFaction.Hostile) {
                bool isDamageable = entity.TryDamage(3);
            }
        }

        // Method called when enemy loses all its health
        public override void Perish() {
            base.Perish();
            Ragdoll();
        }

        public void Ragdoll() {
            rb.isKinematic = false;
            Vector3 force = new Vector3(Random.Range(-0.15f, 0.15f), 0.85f, Random.Range(-0.15f, 0.15f)) * Random.Range(250, 300);
            rb.AddForce(force);
            Vector3 torque = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)) * Random.Range(250, 300);
            rb.AddTorque(torque);
            DetachModules();
            enabled = false;
            Destroy(gameObject, 2);
        }
    }
}
