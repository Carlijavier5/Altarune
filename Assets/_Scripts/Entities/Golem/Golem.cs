using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Golem : Entity {

    public override float TimeScale {
        get => base.TimeScale;
        set {
            timeScale = value;
            foreach (Oscillator oscillator in oscillators) {
                oscillator.SetTimeScale(timeScale);
                navMeshAgent.speed = baseLinearSpeed * timeScale;
                navMeshAgent.angularSpeed = baseAngularSpeed * timeScale;
            }
        }
    }

    private readonly StateMachine<Golem_Input> stateMachine = new();

    [Header("General")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private AggroRange aggroRange;

    private IEnumerable<Rigidbody> rigidbodies;
    private IEnumerable<Oscillator> oscillators;
    private float baseLinearSpeed, baseAngularSpeed;

    void Awake() {
        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = navMeshAgent.angularSpeed;
        rigidbodies = GetComponentsInChildren<Rigidbody>(true).Where((rb) => rb.gameObject != gameObject);
        oscillators = GetComponentsInChildren<Oscillator>(true);

        controller.enabled = false;
        Golem_Input input = new Golem_Input(stateMachine, this);
        stateMachine.Init(input, new State_Idle());

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        aggroRange.OnAggroExit += AggroRange_OnAggroExit;
    }

    private void AggroRange_OnAggroEnter(Entity entity) {
        stateMachine.StateInput.SetTarget(entity);
        stateMachine.SetState(new State_AggroWait());
    }

    private void AggroRange_OnAggroExit(Entity entity) {
        stateMachine.StateInput.SetTarget(null);
        stateMachine.SetState(new State_Idle());
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)
            && entity.Faction != EntityFaction.Hostile) {
            entity.TryDamage(4);
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
        Destroy(this);
    }

    public override void Perish() {
        base.Perish();
        Ragdoll();
    }
}