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
    [SerializeField] private Collider contactCollider;
    [SerializeField] private Material flashMat;
    [SerializeField] private float flashTime;

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

    private int health = 3;

    public void TakeDamage() => StartCoroutine(ITakeDamage());
    private IEnumerator ITakeDamage() {
        contactCollider.enabled = false;
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        Dictionary<Renderer, Material[]> matDict = new();
        foreach (Renderer renderer in renderers) {
            matDict[renderer] = renderer.sharedMaterials;
            renderer.sharedMaterial = flashMat;
        }
        yield return new WaitForSeconds(flashTime);
        foreach (KeyValuePair<Renderer, Material[]> kvp in matDict) {
            kvp.Key.sharedMaterials = kvp.Value;
        }
        contactCollider.enabled = true;
        if (--health <= 0) {
            Ragdoll();
        }
    }
}