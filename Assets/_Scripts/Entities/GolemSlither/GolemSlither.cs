using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class GolemSlither : Entity {

    private enum SlitherAttack { Sweep = 0, Zig = 1 }
    private const string WALK_SPEED_PARAM = "WalkSpeed";

    [Header("Setup")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private AggroRange sweepRange, aggroRange,
                                        deAggroRange;

    private readonly StateMachine<Slither_Input> stateMachine = new();

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

    private void Awake() {
        transform.SetParent(null);

        OnTimeScaleSet += GolemSlither_OnTimeScaleSet;
        OnRootSet += GolemSlither_OnRootSet;
        OnStunSet += GolemSlither_OnStunSet;

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        deAggroRange.OnAggroExit += DeAggroRange_OnAggroExit;

        sweepRange.OnAggroEnter += SweepRange_OnAggroEnter;
        slitherSweep.OnCooldownEnd += TrySweep;
        slitherSweep.OnSweepEnd += SlitherSweep_OnSweepEnd;
        slitherZig.OnWarningComplete += SlitherZig_OnWarningComplete;

        baseAnimatorSpeed = animator.speed;
        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = navMeshAgent.angularSpeed;

        speedParam = Animator.StringToHash(WALK_SPEED_PARAM);

        Slither_Input input = new(stateMachine, this);
        stateMachine.Init(input, new State_Idle());
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
        animator.SetFloat(speedParam, navMeshAgent.velocity.magnitude
                                      / Mathf.Max(1, baseLinearSpeed));
    }

    private void UpdateAggro() {
        Entity currentTarget = stateMachine.StateInput.aggroTarget;
        if (!currentTarget || !deAggroRange.AggroTargets.Contains(currentTarget)) {
            if (aggroRange.HasTarget) {
                Entity closestTarget = aggroRange.ClosestTarget;
                stateMachine.StateInput.SetAggroTarget(closestTarget);
                stateMachine.SetState(new State_Follow());
            } else {
                stateMachine.SetState(new State_Idle()); 
            }
        }
    }

    private void AggroRange_OnAggroEnter(Entity _) => UpdateAggro();
    private void DeAggroRange_OnAggroExit(Entity entity) {
        if (entity == stateMachine.StateInput.aggroTarget) {
            stateMachine.StateInput.SetAggroTarget(null);
        } UpdateAggro();
    }

    private void SweepRange_OnAggroEnter(Entity _) => TrySweep();
    private void TrySweep() {
        if ((stateMachine.State is State_Idle
            || stateMachine.State is State_Chase
            || stateMachine.State is State_Follow)
                && sweepRange.HasTarget) {
            Entity target = sweepRange.ClosestTarget;
            stateMachine.StateInput.SetAggroTarget(target);
            stateMachine.SetState(new State_Sweep());
        }
    }

    private void SlitherSweep_OnSweepEnd() {
        if (stateMachine.State is State_Sweep) {
            stateMachine.SetState(new State_Follow());
        }
    }

    private void SlitherZig_OnWarningComplete() {
        stateMachine.SetState(new State_ZigCharge());
    }

    private void GolemSlither_OnTimeScaleSet(float timeScale) {
        animator.speed = baseAnimatorSpeed * timeScale;
        navMeshAgent.speed = BaseLinearSpeed * timeScale;
        navMeshAgent.angularSpeed = baseAngularSpeed * timeScale;
    }

    private void GolemSlither_OnStunSet(bool isStunned) {
        if (isStunned) stateMachine.SetState(new State_Stun());
        else UpdateAggro();
    }

    private void GolemSlither_OnRootSet(bool canMove) {
        if (!canMove) {
            slitherSweep.CancelSweep();
            if (stateMachine.State is State_ZigAnticipate) {
                slitherZig.CancelZig();
            }
        }
    }

    public override void Perish(bool immediate) {
        base.Perish(immediate);
        DetachModules();
        enabled = false;

        aggroRange.Disable();
        deAggroRange.Disable();
        sweepRange.Disable();

        if (immediate) Destroy(gameObject);
        else Destroy(gameObject, 2);
    }
}

public partial class GolemSlither {

    public class State_Stun : State<Slither_Input> {

        public override void Enter(Slither_Input _) { }

        public override void Update(Slither_Input _) { }

        public override void Exit(Slither_Input input) {
            input.SetAggroTarget(null);
        }
    }
}