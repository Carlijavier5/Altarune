using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    // Accessing the Player and NavMeshAgent
    private Transform player;
    private NavMeshAgent navigation;

    // Initializing the state machine
    private StateMachine<EnemyStateInput> stateMachine;

    // Initializing default values for core components
    [SerializeField] private float health = 50;

    // Initializing default values for movement
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stoppingDistance = 1.5f;

    // Initializing the enemy phases
    private IEnemyActions phaseOneState;
    private IEnemyActions phaseTwoState;
    private IEnemyActions phaseThreeState;

    // First method to run
    void Start() {
        // Initializing NavMeshAgent and Player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navigation = GetComponent<NavMeshAgent>();

        // Initializing variables with Phase files
        phaseOneState = new PhaseOne();
        phaseTwoState = new PhaseTwo();
        phaseThreeState = new PhaseThree();

        // Initializing the state machine
        EnemyStateInput stateInput = new EnemyStateInput(this);
        stateMachine = new StateMachine<EnemyStateInput>();
        stateMachine.Init(stateInput, phaseOneState);

        // Setting the initial phase as PhaseOne
        SetState(phaseOneState);
    }

    // Runs every frame
    void Update() {
        stateMachine.Update();
    }

    // Adds auto-implemented properties (read-only, accessible in other files)
    public NavMeshAgent Navigation => navigation;
    public Transform Player => player;

    public float Health => health;
    public float Speed => speed;
    public float StoppingDistance => stoppingDistance;
}
