using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Golem : Entity {

    private StateMachine<Golem_Input> stateMachine = new();

    [Header("General")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private IEnumerable<Rigidbody> rigidbodies;
    private IEnumerable<Oscillator> oscillators;

    void Awake() {
        rigidbodies = GetComponentsInChildren<Rigidbody>(true).Where((rb) => rb.gameObject != gameObject);
        oscillators = GetComponentsInChildren<Oscillator>(true);

        controller.enabled = false;
        Golem_Input input = new Golem_Input(stateMachine, this);
        stateMachine.Init(input, new State_Idle());
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
    }

    public void Ragdoll() {
        foreach (Oscillator osc in oscillators) osc.enabled = false;
        foreach (Rigidbody rb in rigidbodies) {
            rb.isKinematic = false;
            Vector3 force = new Vector3(Random.Range(-0.15f, 0.15f), 0.85f, Random.Range(-0.15f, 0.15f)) * Random.Range(250, 300);
            rb.AddForce(force);
            Vector3 torque = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)) * Random.Range(250, 300);
            rb.AddTorque(torque);
        } Destroy(this);
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            stateMachine.StateInput.SetTarget(player);
            stateMachine.SetState(new State_AggroWait());
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Player _)) {
            stateMachine.StateInput.SetTarget(null);
            stateMachine.SetState(new State_Idle());
        }
    }
}