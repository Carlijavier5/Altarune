using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class GolemSlither : Entity {

    private enum SlitherAttack { Sweep = 0, ZigZag = 1 }

    [Header("Setup")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private AggroRange sweepRange, aggroRange,
                                        deAggroRange;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform zigZagPoint;

    [Header("Attacks")]
    [SerializeField] private SlitherSweep slitherSweep;
    [SerializeField] private GolemSlither_ZigHitbox zigHitbox1, zigHitbox2,
                                                    zigHitbox3;

    [Header("Attributes")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float sweepCooldownTime = 1f;
    [SerializeField] private float zigCooldownTime = 1f;

    private readonly StateMachine<Slither_Input> stateMachine = new();
    private float baseLinearSpeed, baseAngularSpeed;

    private Entity player;

    private void Awake() {
        OnTimeScaleSet += GolemSlither_OnTimeScaleSet;
        OnRootSet += GolemSlither_OnRootSet;
        OnStunSet += GolemSlither_OnStunSet;

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        deAggroRange.OnAggroExit += DeAggroRange_OnAggroExit;

        sweepRange.OnAggroEnter += SweepRange_OnAggroEnter;
        slitherSweep.OnCooldownEnd += TrySweep;

        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = navMeshAgent.angularSpeed;

        controller.enabled = false;

        player = FindAnyObjectByType<Player>();

        Slither_Input input = new(stateMachine, this);
        stateMachine.Init(input, new State_Idle());
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
    }

    private void TryAttack() {
        if (stateMachine.StateInput.aggroTarget) {
            SlitherAttack attackType = (SlitherAttack) Random.Range(0, 2);
            if (attackType == SlitherAttack.Sweep
                && slitherSweep.Available) {
                Vector3 dir = stateMachine.StateInput.aggroTarget.transform.position
                            - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(dir, Vector3.up);
                slitherSweep.DoSweep(this, lookRotation);
            } else if (attackType == SlitherAttack.ZigZag
                       && (false)) {
                /// Do Zig Zag here
            }
        } else {
            UpdateAggro();
        }
    }

    private void UpdateAggro() {
        
    }

    private void AggroRange_OnAggroEnter(Entity _) => UpdateAggro();
    private void DeAggroRange_OnAggroExit(Entity _) => UpdateAggro();

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

    private void GolemSlither_OnTimeScaleSet(float timeScale) {
        navMeshAgent.speed = baseLinearSpeed * timeScale;
        navMeshAgent.angularSpeed = baseAngularSpeed * timeScale;
    }

    private void GolemSlither_OnStunSet(bool isStunned) {
        if (isStunned) { }///stateMachine.SetState(new State_Stun());
        else UpdateAggro();
    }

    private void GolemSlither_OnRootSet(bool canMove) {
        if (!canMove) {
            /// Cancel Sweep or Zig Zag
        }
    }

    #region Behavior
    public void Aggro() {
        if (stateMachine.State == new State_ChargingZigZag()) {
            return;
        }

        int rand = UnityEngine.Random.Range(0, 2);
        if (rand == 1) {
            stateMachine.SetState(new State_Chase());
        }
        else {
            stateMachine.SetState(new State_ChargingZigZag());
        }
    }

    private void DecideAggro() {
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) <= chaseDistance) {
            Aggro();
        }
        else {
            stateMachine.SetState(new State_Follow());
        }
    }

    public void Ragdoll() {
        DetachModules();
        /// Disable ranges;
        Destroy(gameObject, 2);
    }

    public override void Perish() {
        base.Perish();
        Ragdoll();
    }

    private void Zig(Vector3 newPosition1, Vector3 newPosition2, Vector3 newPosition3) {
        StartCoroutine(IZig(newPosition1, newPosition2, newPosition3));
    }
    #endregion

    #region Coroutines

    private IEnumerator IZig(Vector3 newPosition1, Vector3 newPosition2, Vector3 newPosition3) {
        shouldChange = false;
        Vector3 yOffset = new(0, zigZagPoint.transform.position.y, 0);
        Vector3 originalPosition = controller.transform.position;

        Vector3 position;
        Quaternion rotation;
        float zScale;

        controller.transform.position = newPosition1;

        position = (originalPosition + newPosition1) / 2.0f + yOffset;
        rotation = Quaternion.LookRotation(newPosition1 - originalPosition);
        zScale = (newPosition1 - originalPosition).magnitude;
        zigHitbox1.Generate(position, rotation, zScale);

        yield return new WaitForSeconds(0.15f / TimeScale);

        controller.transform.position = newPosition2;

        position = (newPosition1 + newPosition2) / 2.0f + yOffset;
        rotation = Quaternion.LookRotation(newPosition2 - newPosition1);
        zScale = (newPosition2 - newPosition1).magnitude;
        zigHitbox2.Generate(position, rotation, zScale);

        yield return new WaitForSeconds(0.15f / TimeScale);

        controller.transform.position = newPosition3;

        position = (newPosition2 + newPosition3) / 2.0f + yOffset;
        rotation = Quaternion.LookRotation(newPosition3 - newPosition2);
        zScale = (newPosition3 - newPosition2).magnitude;
        zigHitbox3.Generate(position, rotation, zScale);

        yield return new WaitForSeconds(0.7f / TimeScale);

        if (zigHitbox1 != null) zigHitbox1.Detonate();
        if (zigHitbox2 != null) zigHitbox2.Detonate();
        if (zigHitbox3 != null) zigHitbox3.Detonate();

        shouldChange = true;
        didSweep = false;
        stateMachine.SetState(new State_Idle());
    }
    #endregion
}

public partial class GolemSlither {

    public class State_Stun : State<Slither_Input> {

        public override void Enter(Slither_Input input) { }

        public override void Update(Slither_Input input) { }

        public override void Exit(Slither_Input input) { }
    }
}