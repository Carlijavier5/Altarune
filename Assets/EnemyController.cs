using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    // Accessing the Player and NavMeshAgent
    public Transform player;
    public NavMeshAgent navigation;

    // Initializing default values for core components
    [SerializeField] private float health = 50;

    // Initializing default values for movement
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stoppingDistance = 1.5f;

    // Initializing the enemy phases
    private IEnemyActions currentState;
    private IEnemyActions phaseOneState;
    private IEnemyActions phaseTwoState;
    private IEnemyActions phaseThreeState;

    // Initializing the minion prefab and array
    [SerializeField] private GameObject minionPrefab;
    private GameObject[] minions;

    // First method to run
    void Start() {
        // Initializing variables with Phase files
        phaseOneState = new PhaseOne();
        phaseTwoState = new PhaseTwo();
        phaseThreeState = new PhaseThree();

        // Spawns the minions
        InitializeMinions();

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

    // Method to initialize the minions
    private void InitializeMinions() {
        // Spawns between 4 and 6 minions
        int numMinions = Random.Range(4, 6);
        minions = new GameObject[numMinions];

        // Minions are spawned at (0, 0, 0), but hidden
        for (int i = 0; i < numMinions; i++) {
            minions[i] = Instantiate(minionPrefab, Vector3.zero, Quaternion.identity);
            minions[i].SetActive(false);
        }
    }

    // Adds auto-implemented properties (read-only, accessible in other files)
    public NavMeshAgent Navigation => navigation;
    public Transform Player => player;

    public float Health => health;

    public float Speed => speed;
    public float StoppingDistance => stoppingDistance;

    public GameObject[] Minions => minions;
}
