using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SavagePhase { Phase1 = 0, Phase2 = 1, Phase3 = 2 }

public partial class GolemSavage : Entity {

    public event System.Action<int> OnPhaseTransition;

    private const string WALK_SPEED_PARAM = "WalkSpeed";

    [Header("General")]
    [SerializeField] private Animator animator;
    [SerializeField] private SavageCutsceneManager cutsceneManager;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private AggroRange aggroRange;
    [SerializeField] private Collider contactCollider;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private SavagePhaseConfiguration[] phaseConfigurations;

    private readonly StateMachine<Savage_Input> macroMachine = new();
    private readonly StateMachine<Savage_Input> microMachine = new();
    private readonly Dictionary<SavagePhase, SavagePhaseConfiguration> configMap = new();

    private SavagePhaseConfiguration activeConfig;
    private SavagePhase stagingPhase;

    private float baseLinearSpeed;
    public float BaseLinearSpeed {
        get => baseLinearSpeed;
        set {
            baseLinearSpeed = value;
            navMeshAgent.speed = baseLinearSpeed;
        }
    }
    private int speedParam;

    void Awake() {
        cutsceneManager.OnCutsceneEnd += CutsceneManager_OnCutsceneEnd;
        OnDamageReceived += GolemSavage_OnDamageReceived;

        speedParam = Animator.StringToHash(WALK_SPEED_PARAM);
        foreach (SavagePhaseConfiguration config in phaseConfigurations) {
            configMap[config.savagePhase] = config;
        }

        foreach (GolemSiftling tornadoSiftling in earthTornadoSiftlings) {
            tornadoSiftling.OnPerish += TornadoSiftling_OnPerish;
        }

        Savage_Input stateInput = new(macroMachine, microMachine, this);
        macroMachine.Init(stateInput, new State_Inert());
        microMachine.Init(stateInput, new State_Inert());

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        aggroRange.OnAggroExit += AggroRange_OnAggroExit;
    }

    void OnEnable() => StartCoroutine(AwaitActivation());
    private IEnumerator AwaitActivation() {
        yield return new WaitForSeconds(initDelay);
        while (!actorInRange) yield return null;

        Transform[] bodyTransforms = objectBody.GetComponentsInChildren<Transform>(true);
        foreach (Transform bodyTransform in bodyTransforms) {
            bodyTransform.gameObject.layer = LayerUtils.HostileLayer; 
        }
       
        aggroRange.Disable();
        cutsceneManager.Activate();
    }

    protected override void Update() {
        base.Update();
        macroMachine.Update();
        microMachine.Update();
        animator.SetFloat(speedParam, navMeshAgent.velocity.magnitude
                                      / Mathf.Max(1, baseLinearSpeed));
    }

    private void CutsceneManager_OnCutsceneEnd() {
        contactCollider.enabled = true;
        stagingPhase = SavagePhase.Phase1;
        DoPhaseTransition();
        float attackTime = activeConfig.RandomAttackTime;
        microMachine.SetState(new State_Idle(stagingPhase, attackTime));
    }

    public void DoPhaseTransition() {
        if (configMap.ContainsKey(stagingPhase)) {
            activeConfig = configMap[stagingPhase];
            OnPhaseTransition?.Invoke(activeConfig.health);
        }

        PhaseState nextState = stagingPhase == SavagePhase.Phase1 ? new PhaseState_Phase1()
                             : stagingPhase == SavagePhase.Phase2 ? new PhaseState_Phase2()
                             : stagingPhase == SavagePhase.Phase3 ? new PhaseState_Phase3()
                             : null;
        if (nextState != null) macroMachine.SetState(nextState);
    }

    public void Animator_OnSpinEnd() {
        if (microMachine.State is State_Spin
                || microMachine.State is State_EarthSpin) {
            PhaseState phaseState = macroMachine.State as PhaseState;
            float attackTime = activeConfig.RandomAttackTime;
            microMachine.SetState(new State_Idle(phaseState.Phase, attackTime));
        }
    }

    public void Animator_OnSlamHit() {
        slamWave.DoSlam(transform.position, activeConfig.slamDuration);
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)
                && !baseObject.IsFaction(EntityFaction.Hostile)) {
            baseObject.TryDamage(3);
        }
    }

    private void GolemSavage_OnDamageReceived(int _) {
        if (Health <= 0) {
            PhaseState phaseState = macroMachine.State as PhaseState;
            if (phaseState.Phase == SavagePhase.Phase2
                && stagingPhase == SavagePhase.Phase2) {
                State_EarthSpin spinState = microMachine.State as State_EarthSpin;
                spinState.Stop();
                stagingPhase = SavagePhase.Phase3;
            } if (phaseState.Phase == stagingPhase) {
                if (phaseState.Phase == SavagePhase.Phase3) {
                    Perish();
                } else stagingPhase++;
            }
        }
    }

    private void TornadoSiftling_OnPerish(BaseObject _) => TryBypassIFrame(1);

    private void AggroRange_OnAggroEnter(Entity entity) {
        if (entity is Player) {
            macroMachine.StateInput.SetAggroTarget(entity);
            actorInRange = true;
        }
    }

    private void AggroRange_OnAggroExit(Entity entity) {
        if (entity is Player) actorInRange = false;
    }

    public override void Perish() {
        base.Perish();
        macroMachine.SetState(new State_Inert());
        microMachine.SetState(new State_Perish());
        DetachModules();
        enabled = false;
        Ragdoll();
        Destroy(gameObject, 2);
    }

    public void Ragdoll() {
        rb.useGravity = true;
        rb.isKinematic = false;
        Vector3 force = new Vector3(Random.Range(-0.15f, 0.15f), 0.85f, Random.Range(-0.15f, 0.15f)) * Random.Range(250, 300);
        rb.AddForce(force);
        Vector3 torque = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)) * Random.Range(250, 300);
        rb.AddTorque(torque);
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        using (new UnityEditor.Handles.DrawingScope()) {
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up,
                                             siftlingSpawnRadius.x);
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up,
                                             siftlingSpawnRadius.y);
        }
    }
    #endif
}