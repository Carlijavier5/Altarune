using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SiftlingType { Normal, Fire, Wind, Water }

public partial class GolemSiftling : Entity {

    private const string WALK_SPEED_PARAM = "WalkSpeed";

    public event System.Action<int> OnAscend;

    [SerializeField] private Animator animator;
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
                           * status.timeScale;
        }
    }

    private float baseLinearSpeed;
    private float BaseLinearSpeed {
        get => baseLinearSpeed;
        set {
            baseLinearSpeed = value;
            navMeshAgent.speed = baseLinearSpeed
                               * status.timeScale
                               * RootMult;
        }
    }

    private float baseAngularSpeed;

    private int speedParam;

    void Awake() {
        OnDamageReceived += GolemSiftling_OnDamageReceived;
        OnStunSet += GolemSiftling_OnStunSet;
        OnRootSet += GolemSiftling_OnRootSet;
        OnTimeScaleSet += GolemSiftling_OnTimeScaleSet;

        foreach (SiftlingConfiguration config in configurations) {
            configMap[config.type] = config;
            if (config.tornado) {
                config.tornado.OnTornadoSummoned += Tornado_OnTornadoSummoned;
            }
        }

        baseAnimatorSpeed = animator.speed;
        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = navMeshAgent.angularSpeed;
        activeConfig = configMap[SiftlingType.Normal];

        speedParam = Animator.StringToHash(WALK_SPEED_PARAM);

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
        navMeshAgent.speed = BaseLinearSpeed * status.timeScale * RootMult;
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