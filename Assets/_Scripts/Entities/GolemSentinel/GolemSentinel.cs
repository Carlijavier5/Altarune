using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class GolemSentinel : Entity {

    private const string WALK_SPEED_PARAM = "WalkSpeed";

    private readonly StateMachine<Sentinel_Input> stateMachine = new();

    [Header("General")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private AggroRange aggroRange, deAggroRange;
    [SerializeField] private Collider attackCollider;

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
        OnTimeScaleSet += Golem_OnTimeScaleSet;
        OnRootSet += Golem_OnRootSet;
        OnStunSet += Golem_OnStunSet;

        sentinelSweep.OnSweepEnd += SentinelSweep_OnSweepEnd;

        baseAnimatorSpeed = animator.speed;
        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = navMeshAgent.angularSpeed;

        speedParam = Animator.StringToHash(WALK_SPEED_PARAM);

        controller.enabled = false;
        Sentinel_Input input = new(stateMachine, this);
        stateMachine.Init(input, new State_Idle());

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        deAggroRange.OnAggroExit += DeAggroRange_OnAggroExit;
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
        animator.SetFloat(speedParam, navMeshAgent.velocity.magnitude
                                      / Mathf.Max(1, baseLinearSpeed));
    }

    private void ClearContacts() {
        StartCoroutine(IClearContacts());
    }

    private IEnumerator IClearContacts() {
        attackCollider.enabled = false;
        yield return new WaitForEndOfFrame();
        attackCollider.enabled = true;
    }

    private void SentinelSweep_OnSweepEnd() {
        stateMachine.SetState(new State_Idle());
        UpdateAggro();
    }

    private void AggroRange_OnAggroEnter(Entity _) => UpdateAggro();
    private void DeAggroRange_OnAggroExit(Entity _) => UpdateAggro();

    private void UpdateAggro() {
        if (stateMachine.State is State_Stun
            || stateMachine.State is State_Charging
            || stateMachine.State is State_Charge
            || stateMachine.State is State_Sweep) return;

        Entity closestTarget = aggroRange.ClosestTarget;
        stateMachine.StateInput.SetTarget(closestTarget);

        if (closestTarget != null) {
            stateMachine.SetState(new State_AggroWait());
        } else {
            stateMachine.SetState(new State_Idle());
        }
    }

    private void Golem_OnRootSet(bool canMove) {
        navMeshAgent.speed = BaseLinearSpeed * status.timeScale * RootMult;
        if (stateMachine.State is State_Sweep) sentinelSweep.CancelSweep();
    }

    private void Golem_OnStunSet(bool isStunned) {
        State<Sentinel_Input> newState = isStunned ? new State_Stun()
                                                   : new State_Idle();
        stateMachine.SetState(newState);
        if (!isStunned) UpdateAggro();
    }

    private void Golem_OnTimeScaleSet(float timeScale) {
        animator.speed = baseAnimatorSpeed * timeScale;
        navMeshAgent.speed = BaseLinearSpeed * timeScale * RootMult;
        navMeshAgent.angularSpeed = baseAngularSpeed * timeScale * RootMult;
    }

    void OnTriggerEnter(Collider other) {
        if (stateMachine.State is not State_Charge) return;
        if (other.TryGetComponent(out BaseObject baseObject)) {
            Entity entity = baseObject as Entity;
            if (entity == null || entity.Faction != EntityFaction.Hostile) {
                baseObject.TryDamage(damageAmount);
            }
        }
    }

    public override void Perish() {
        base.Perish();
        DetachModules();
        enabled = false;
        aggroRange.Disable();
        Destroy(gameObject, 2);
    }
}