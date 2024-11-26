using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GolemSavage {
    public partial class GolemSavage : Entity {
        // Creating the NavMeshAgent, Player, Damageable, and the minionPrefabs
        private NavMeshAgent navigation;
        private Transform player;
        private Damageable damageable;
        [SerializeField] private GameObject fireMinionPrefab;
        [SerializeField] private GameObject waterMinionPrefab;
        [SerializeField] private GameObject windMinionPrefab;

        // Initializing default values for movement
        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float stoppingDistance = 1f;
        [SerializeField] private Rigidbody rb;
        public float RootMult => CanMove ? 1 : 0;
        
        // Creating health variable
        private float health;

        // Creating the state machine
        private StateMachine<GolemSavageStateInput> stateMachine;

        // Creating the enemy phases
        private GolemSavage_PhaseOne phaseOneState;
        private GolemSavage_PhaseTwo phaseTwoState;
        private GolemSavage_PhaseThree phaseThreeState;
        private GolemSavage_Tornado tornado;
        private GolemSavage_GroundSlam groundSlam;
        private GolemSavage_MeteorStrike meteorStrike;
        private GolemSavage_Stunned stunnedPhase;

        private bool active;

        public void Start() {
            // Initializes base components
            navigation = GetComponent<NavMeshAgent>();
            damageable = GetComponent<Damageable>();
        }

        void OnEnable() => TryStart();

        private void TryStart() {
            Start();

            if (FindPlayer()) {
                active = true;

                // Initializes health
                health = damageable.Health;

                // Initializing variables with Phase files
                phaseOneState = new GolemSavage_PhaseOne();
                phaseThreeState = new GolemSavage_PhaseThree();

                // Initializing the state machine and setting initial phase
                stateMachine = new StateMachine<GolemSavageStateInput>();
                GolemSavageStateInput stateInput = new GolemSavageStateInput(this);
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
                stateMachine.SetState(new GolemSavage_Stunned());
            } else {
                // Logic to switch back to current state
                if (health > 70) {
                    stateMachine.SetState(new GolemSavage_PhaseOne());
                }
                stateMachine.SetState(new GolemSavage_PhaseThree());
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
