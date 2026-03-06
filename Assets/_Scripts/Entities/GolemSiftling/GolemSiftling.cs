using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SiftlingType { Normal, Fire, Wind, Water }

public partial class GolemSiftling : Entity {

    private const string WALK_SPEED_PARAM = "WalkSpeed";

    public event System.Action<int> OnAscend;

    [SerializeField] private Animator animatorMain;
    [SerializeField] private Animator animatorBody;
    [SerializeField] private AggroRange aggroRange, deAggroRange;
    [SerializeField] private SiftlingType[] availableAscensions;
    [SerializeField] private SiftlingConfiguration[] configurations;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private readonly StateMachine<Siftling_Input> stateMachine = new();

    private readonly Dictionary<SiftlingType, SiftlingConfiguration> configMap = new();
    private SiftlingConfiguration activeConfig;

    private SiftlingType ascensionType;

    private float baseMainAnimatorSpeed;
    public float BaseMainAnimatorSpeed {
        get => baseMainAnimatorSpeed;
        set {
            baseMainAnimatorSpeed = value;
            animatorMain.speed = baseMainAnimatorSpeed
                               * Status.timeScale;
        }
    }
    private float baseBodyAnimatorSpeed;

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
        }

        idleStoppingDistance = navMeshAgent.stoppingDistance;
        idleAngularSpeed = navMeshAgent.angularSpeed;

        baseMainAnimatorSpeed = animatorMain.speed;
        baseBodyAnimatorSpeed = animatorBody.speed;
        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = idleAngularSpeed;

        activeConfig = configMap[SiftlingType.Normal];
        ascensionType = ChooseAscension();

        speedParam = Animator.StringToHash(WALK_SPEED_PARAM);
        RestartAttackCooldown();

        stateMachine.Init(new(stateMachine, this), new State_Idle());
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
        float speedVal = activeConfig != null && activeConfig.type == SiftlingType.Wind
                       ? 0 : navMeshAgent.velocity.magnitude / Mathf.Max(1, baseLinearSpeed);
        animatorMain.SetFloat(speedParam, speedVal);

        if (Input.GetKeyDown(KeyCode.O)) {
            TryDamage(100);
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            DoAscension(false);
        }
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
        if (stateMachine.State is not State_FireWindUp) {
            animatorMain.speed = baseMainAnimatorSpeed * timeScale;
        }

        navMeshAgent.speed = BaseLinearSpeed * timeScale * RootMult;
        navMeshAgent.angularSpeed = baseAngularSpeed * timeScale * RootMult;
    }

    private void GolemSiftling_OnDamageReceived(int _) {
        if (Health <= 0) DoAscension(true);
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

    private SiftlingType ChooseAscension() {
        int typeIndex = Random.Range(0, availableAscensions.Length);
        return availableAscensions[typeIndex];
    }

    private void DoAscension(bool isFirst) {
        if (ascensionType != SiftlingType.Normal
                && configMap.TryGetValue(ascensionType, out activeConfig)) {
            OnAscend?.Invoke(activeConfig.health);
            stateMachine.SetState(new State_Ascend(isFirst));

            switch (ascensionType) {
                case SiftlingType.Wind:
                    OnTryLongPush += GolemSiftling_OnTryLongPush;
                    break;
            }
        } else if (isFirst) {
            Perish();
        }
    }

    private void GolemSiftling_OnTryLongPush(Vector3 strength, float duration,
                                             EventResponse<PushActionCore> response) {
        response.objectReference.Kill();
        OnTryLongPush -= GolemSiftling_OnTryLongPush;
        TryLongPush(strength, pushPhaseStrength, bounceDuration, out PushActionCore core);
        core.SetEase(EaseCurve.InLogarithmic);
        stateMachine.SetState(new State_WindDescent());
        OnTryLongPush += GolemSiftling_OnTryLongPush;
    }

    public void FlipTornadoDirection() {
        TornadoDirectionMultiplier = TornadoDirectionMultiplier < 0 ? 1 : -1;
    }

    public void RestartAttackCooldown() {
        if (activeConfig.attackCDRange.sqrMagnitude <= 0) {
            canAttackTime = Mathf.Infinity;
        } else {
            canAttackTime = Time.time + Random.Range(activeConfig.attackCDRange.x, activeConfig.attackCDRange.y);
        }
    }

    public override void Perish(bool immediate = false) {
        base.Perish(immediate);
        DetachModules();

        if (immediate) {
            Destroy(gameObject);
        } else {
            enabled = false;

            switch (activeConfig.type) {
                case SiftlingType.Wind:
                    OnTryLongPush -= GolemSiftling_OnTryLongPush;
                    break;
            }

            Destroy(gameObject, 2);
        }
    }
}