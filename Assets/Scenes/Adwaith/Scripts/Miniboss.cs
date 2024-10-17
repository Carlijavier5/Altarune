using UnityEngine;
using UnityEngine.AI;

namespace Miniboss {
    public partial class Miniboss : MonoBehaviour {
        // Accessing the NavMeshAgent, Player, Damageable, and the minionPrefab
        private NavMeshAgent navigation;
        private Transform player;
        private Damageable damageable;
        [SerializeField] private GameObject minionPrefab;

        // Initializing default values for movement
        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float stoppingDistance = 1.5f;
        [SerializeField] private float health = 100;

        // Initializing the state machine
        private StateMachine<MinibossStateInput> stateMachine;

        // Initializing the enemy phases
        private PhaseOne phaseOneState;
        private PhaseTwo phaseTwoState;
        private PhaseThree phaseThreeState;

        // First method to run
        public void Start() {
            // Initializes base components
            navigation = GetComponent<NavMeshAgent>();
            damageable = GetComponent<Damageable>();
            FindPlayer();

            // Initializing variables with Phase files
            phaseOneState = new PhaseOne();
            phaseTwoState = new PhaseTwo();
            phaseThreeState = new PhaseThree();

            // Initializing the state machine and setting initial phase
            stateMachine = new StateMachine<MinibossStateInput>();
            MinibossStateInput stateInput = new MinibossStateInput(this);
            stateMachine.Init(stateInput, phaseOneState);
        }

        // Runs every frame
        public void Update() {
            stateMachine.Update();
        }

        // Finds the player, and assigns it to the Transform player
        public void FindPlayer() {
            // Initializes the OverlapSphere collider
            Collider[] findPlayerCollider = Physics.OverlapSphere(
                transform.position, 
                10f, 
                LayerMask.GetMask("Player")
            );

            // If the player collider is found, assigns it to the transform
            if (findPlayerCollider != null) {
                player = findPlayerCollider[0].transform;
            }
        }
    }
}
