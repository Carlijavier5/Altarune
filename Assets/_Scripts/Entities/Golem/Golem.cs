using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public partial class Golem : Entity {

    private readonly StateMachine<Golem_Input> stateMachine = new();

    [Header("General")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private AggroRange aggroRange;

    public float RootMult => CanMove ? 1 : 0;

    private IEnumerable<Rigidbody> rigidbodies;
    private IEnumerable<Oscillator> oscillators;
    private float baseLinearSpeed, baseAngularSpeed;

    void Awake() {
        OnTimeScaleSet += Golem_OnTimeScaleSet;
        OnStunSet += Golem_OnStunSet;
        OnRootSet += Golem_OnRootSet;

        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = navMeshAgent.angularSpeed;
        rigidbodies = GetComponentsInChildren<Rigidbody>(true).Where((rb) => rb.gameObject != gameObject);
        oscillators = GetComponentsInChildren<Oscillator>(true);

        controller.enabled = false;
        Golem_Input input = new(stateMachine, this);
        stateMachine.Init(input, new State_Idle());

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        aggroRange.OnAggroExit += AggroRange_OnAggroExit;
    }

    private void Golem_OnRootSet(bool canMove) {
        navMeshAgent.speed = baseLinearSpeed * status.timeScale * RootMult;
    }

    private void Golem_OnStunSet(bool isStunned) {
        State<Golem_Input> newState = isStunned ? new State_Stunned()
                                                : new State_Idle();
        stateMachine.SetState(newState);
        UpdateAggro();
    }

    private void Golem_OnTimeScaleSet(float timeScale) {
        animator.speed = timeScale;
        foreach (Oscillator oscillator in oscillators) {
            oscillator.SetTimeScale(timeScale);
        }
        navMeshAgent.speed = baseLinearSpeed * timeScale * RootMult;
        navMeshAgent.angularSpeed = baseAngularSpeed * timeScale * RootMult;
    }

    private void AggroRange_OnAggroEnter(Entity _) => UpdateAggro();

    private void AggroRange_OnAggroExit(Entity _) => UpdateAggro();

    private void UpdateAggro() {
        if (stateMachine.State is State_Stunned) return;
        if (aggroRange.AggroTargets.Count > 0) {
            Entity closestTarget = aggroRange.AggroTargets.First();
            float closestDistance = Vector3.Distance(closestTarget.transform.position,
                                                     transform.position);
            foreach (Entity target in aggroRange.AggroTargets) {
                float newDistance = Vector3.Distance(target.transform.position,
                                                     transform.position);
                if (newDistance < closestDistance) {
                    closestTarget = target;
                    closestDistance = newDistance;
                }
            }

            stateMachine.StateInput.SetTarget(closestTarget);
            stateMachine.SetState(new State_AggroWait());
        } else {
            stateMachine.StateInput.SetTarget(null);
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

    public void Ragdoll() {
        foreach (Oscillator osc in oscillators) osc.enabled = false;
        foreach (Rigidbody rb in rigidbodies) {
            rb.isKinematic = false;
            Vector3 force = new Vector3(Random.Range(-0.15f, 0.15f), 0.85f, Random.Range(-0.15f, 0.15f)) * Random.Range(250, 300);
            rb.AddForce(force);
            Vector3 torque = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)) * Random.Range(250, 300);
            rb.AddTorque(torque);
        } DetachModules();
        enabled = false;
        Destroy(gameObject, 2);
    }

    public override void Perish() {
        base.Perish();
        Ragdoll();
    }
}