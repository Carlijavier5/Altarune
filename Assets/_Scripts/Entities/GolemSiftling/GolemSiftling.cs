using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SiftlingType { Normal, Fire, Wind, Water }

public partial class GolemSiftling : Entity {

    private const string WALK_SPEED_PARAM = "WalkSpeed";

    public event System.Action<int> OnAscend;

    [SerializeField] private Animator animator;
    [SerializeField] private AggroRange aggroRange, deAggroRange;
    [SerializeField] private SiftlingType[] availableAscensions;
    [SerializeField] private SiftlingConfiguration[] configurations;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private readonly StateMachine<Siftling_Input> stateMachine = new();

    private readonly Dictionary<SiftlingType, SiftlingConfiguration> configMap = new();
    private SiftlingConfiguration activeConfig;

    private float baseAnimatorSpeed;
    public float BaseAnimatorSpeed {
        get => baseAnimatorSpeed;
        set {
            baseAnimatorSpeed = value;
            animator.speed = baseAnimatorSpeed
                           * Status.timeScale;
        }
    }

    private float baseLinearSpeed;
    private float BaseLinearSpeed {
        get => baseLinearSpeed;
        set {
            baseLinearSpeed = value;
            navMeshAgent.speed = baseLinearSpeed
                               * Status.timeScale
                               * RootMult;
        }
    }

    private float baseAngularSpeed;
    private float BaseAngularSpeed {
        get => baseAngularSpeed;
        set {
            baseAngularSpeed = value;
            navMeshAgent.angularSpeed = baseAngularSpeed
                                      * Status.timeScale
                                      * RootMult;
        }
    }

    private float idleStoppingDistance;
    private float idleAngularSpeed;

    private int speedParam;

    void Awake() {
        OnDamageReceived += GolemSiftling_OnDamageReceived;
        OnStunSet += GolemSiftling_OnStunSet;
        OnRootSet += GolemSiftling_OnRootSet;
        OnTimeScaleSet += GolemSiftling_OnTimeScaleSet;

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        deAggroRange.OnAggroExit += DeAggroRange_OnAggroExit;

        foreach (SiftlingConfiguration config in configurations) {
            configMap[config.type] = config;
            if (config.tornado) {
                config.tornado.OnTornadoSummoned += Tornado_OnTornadoSummoned;
            }
        }

        idleStoppingDistance = navMeshAgent.stoppingDistance;
        idleAngularSpeed = navMeshAgent.angularSpeed;

        baseAnimatorSpeed = animator.speed;
        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = idleAngularSpeed;
        activeConfig = configMap[SiftlingType.Normal];

        speedParam = Animator.StringToHash(WALK_SPEED_PARAM);
        RestartChargeCooldown();

        stateMachine.Init(new(stateMachine, this), new State_Idle());
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
        float speedVal = activeConfig != null && activeConfig.type == SiftlingType.Wind
                       ? 0 : navMeshAgent.velocity.magnitude / Mathf.Max(1, baseLinearSpeed);
        animator.SetFloat(speedParam, speedVal);
    }

    private void GolemSiftling_OnStunSet(bool isStunned) {
        if (stateMachine.State is State_Ascend) {
            Perish();
        } else {
            State<Siftling_Input> newState = isStunned ? new State_Stun()
                                                       : new State_Idle();
            stateMachine.SetState(newState);
        }
    }

    private void GolemSiftling_OnRootSet(bool canMove) {
        navMeshAgent.speed = BaseLinearSpeed * Status.timeScale * RootMult;
    }

    private void GolemSiftling_OnTimeScaleSet(float timeScale) {
        animator.speed = baseAnimatorSpeed * timeScale;
        navMeshAgent.speed = BaseLinearSpeed * timeScale * RootMult;
        navMeshAgent.angularSpeed = baseAngularSpeed * timeScale * RootMult;
    }

    private void GolemSiftling_OnDamageReceived(int _) {
        if (Health <= 0) {
            int typeIndex = Random.Range(0, availableAscensions.Length);
            SiftlingType type = availableAscensions[typeIndex];
            if (type != SiftlingType.Normal
                    && configMap.TryGetValue(type, out activeConfig)) {
                availableAscensions = new[] { SiftlingType.Normal };
                OnAscend?.Invoke(activeConfig.health);
                stateMachine.SetState(new State_Ascend());
            } else {
                Perish();
            }
        }
    }

    private void AggroRange_OnAggroEnter(Entity _) => UpdateAggro();
    private void DeAggroRange_OnAggroExit(Entity entity) {
        if (stateMachine.State is State_Aggro) {
            State_Aggro state = stateMachine.State as State_Aggro;
            state.PropagateAggroExit(entity);
        }
        UpdateAggro();
    }

    private void UpdateAggro() {
        Entity closestTarget = aggroRange.ClosestTarget;
        stateMachine.StateInput.SetTarget(closestTarget);
    }

    public void RestartChargeCooldown() {
        canChargeTime = Time.time + Random.Range(chargeCDRange.x, chargeCDRange.y);
    }

    public void Animator_OnAscensionRisen() {
        activeConfig.tornado.Toggle(true);
    }

    public void Tornado_OnTornadoSummoned() {
        string trigger = activeConfig.type == SiftlingType.Wind ? DESCEND_WIND_PARAM
                                                                : DESCEND_PARAM;
        animator.SetTrigger(trigger);
    }

    public void Animator_OnDescent() {
        RemoveMaterial(ascendMaterial);
        if (activeConfig.crystalMaterial) {
            crystalRenderer.sharedMaterial = activeConfig.crystalMaterial;
            UpdateRendererRefs(true);
        }
        stateMachine.SetState(new State_Idle());
    }

    public override void Perish(bool immediate = false) {
        base.Perish(immediate);
        DetachModules();

        if (immediate) {
            Destroy(gameObject);
        } else {
            enabled = false;
            Destroy(gameObject, 2);
        }
    }
}