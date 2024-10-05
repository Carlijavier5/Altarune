using UnityEngine;
using UnityEngine.AI;

namespace Miniboss {
    public partial class Miniboss : MonoBehaviour {
        // Accessing the NavMeshAgent, Player, Damageable, and the minionPrefab
        [SerializeField] private NavMeshAgent navigation;
        [SerializeField] private Transform player;
        [SerializeField] private Damageable damageable;
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
        void Start() {
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
        void Update() {
            stateMachine.Update();
        }
    }
}
