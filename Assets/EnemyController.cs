using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    // Accessing the Player and NavMeshAgent
    public Transform player;
    public NavMeshAgent navigation;

    // Initializing default values for core components
    [SerializeField] private float health = 100;
    [SerializeField] private float attackDmg;
    [SerializeField] private float defense;

    // Initializing default values for movement
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stoppingDistance = 1.5f;

    // Initializing the enemy phases
    private IEnemyActions currentState;
    private IEnemyActions phaseOneState;
    private IEnemyActions phaseTwoState;
    private IEnemyActions phaseThreeState;

    // First method to run
    void Start() {
        // Initializing variables with Phase files
        phaseOneState = new PhaseOne();
        phaseTwoState = new PhaseTwo();
        phaseThreeState = new PhaseThree();

        // Setting the initial phase as PhaseOne
        SetState(phaseOneState);
    }

    // Runs every frame
    void Update() {
        if (currentState != null) {
            // Runs the Execute method in the current phase every frame
            currentState.Execute();
            // Updates state based on health
            CheckStateTransition();
        }
    }

    // Sets the state and enters it
    void SetState(IEnemyActions nextPhase) {
        currentState?.Exit();
        currentState = nextPhase;
        currentState.Enter(this);
    }

    // Updates state based on health
    void CheckStateTransition() {
        if (health <= 0) {
            SetState(null);
        } else if (health <= 50 && health >= 25) {
            SetState(phaseTwoState);
        } else if (health < 25) {
            SetState(phaseThreeState);
        }
    }

    // Adds auto-implemented properties (read-only, accessible in other files)
    public NavMeshAgent Navigation => navigation;
    public Transform Player => player;

    public float Health => health;
    public float AttackDmg => attackDmg;
    public float Defense => defense;

    public float Speed => speed;
    public float StoppingDistance => stoppingDistance;
}
