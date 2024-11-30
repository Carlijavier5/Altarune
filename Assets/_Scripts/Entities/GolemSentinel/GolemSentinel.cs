using System.Collections.Generic;
using System.Linq;
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

    void Awake() {
        OnTimeScaleSet += Golem_OnTimeScaleSet;
        OnStunSet += Golem_OnStunSet;
        OnRootSet += Golem_OnRootSet;

        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = navMeshAgent.angularSpeed;

        controller.enabled = false;
        Sentinel_Input input = new(stateMachine, this);
        stateMachine.Init(input, new State_Idle());

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        deAggroRange.OnAggroExit += DeAggroRange_OnAggroExit;
    }

    private void Golem_OnRootSet(bool canMove) {
        navMeshAgent.speed = BaseLinearSpeed * status.timeScale * RootMult;
    }

    private void Golem_OnStunSet(bool isStunned) {
        State<Sentinel_Input> newState = isStunned ? new State_Stunned()
                                                : new State_Idle();
        stateMachine.SetState(newState);
        if (!isStunned) UpdateAggro();
    }

    private void Golem_OnTimeScaleSet(float timeScale) {
        animator.speed = timeScale;
        navMeshAgent.speed = BaseLinearSpeed * timeScale * RootMult;
        navMeshAgent.angularSpeed = baseAngularSpeed * timeScale * RootMult;
    }

    private void AggroRange_OnAggroEnter(Entity _) => UpdateAggro();

    private void DeAggroRange_OnAggroExit(Entity _) => UpdateAggro();

    private void UpdateAggro() {
        if (stateMachine.State is State_Stunned
            || stateMachine.State is State_Charging
            || stateMachine.State is State_Charge) return;

        Entity closestTarget = aggroRange.ClosestTarget;
        stateMachine.StateInput.SetTarget(closestTarget);

        if (closestTarget != null) {
            stateMachine.SetState(new State_AggroWait());
        } else {
            stateMachine.SetState(new State_Idle());
        }
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
    }

    void OnTriggerEnter(Collider other) {
        if (stateMachine.State is not State_Charge) return;
        if (other.TryGetComponent(out BaseObject baseObject)) {
            Entity entity = baseObject as Entity;
            if (entity == null || entity.Faction != EntityFaction.Hostile) {
                baseObject.TryDamage(4);
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