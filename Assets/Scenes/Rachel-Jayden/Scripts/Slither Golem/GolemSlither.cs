using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public partial class GolemSlither : Entity
{
    [NonSerialized] public readonly StateMachine<GolemSlither_Input> stateMachine = new();

    [Header("Setup")]
    [SerializeField] private GolemSlither_SweepRadius sweepRadius;
    [SerializeField] private GolemSlither_AggroRadius aggroRadius;
    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform sweepPoint;

    private IEnumerable<Rigidbody> rigidbodies;
    private IEnumerable<Oscillator> oscillators;
    private float baseLinearSpeed, baseAngularSpeed;

    private Entity player;

    [Header("Attacks")]
    [SerializeField] private GolemSlither_SweepHitbox sweepHitbox;
    [SerializeField] private GolemSlither_ZigHitbox zigHitbox;

    [Header("Attributes")]
    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float sweepDistance = 2.5f;
    [SerializeField] private float sweepDuration = 1.5f;
    [SerializeField] private float followSpeed = 0.75f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float sweepCooldownTime = 1f;
    [SerializeField] private float zigCooldownTime = 1f;

    [NonSerialized] public bool shouldChange;

    private bool didSweep = false;

    private void Awake() {
        OnTimeScaleSet += GolemSlither_OnTimeScaleSet;

        sweepRadius.GetComponent<SphereCollider>().radius = sweepDistance;
        aggroRadius.GetComponent<SphereCollider>().radius = chaseDistance;

        player = FindAnyObjectByType<Player>();

        rigidbodies = GetComponentsInChildren<Rigidbody>(true).Where((rb) => rb.gameObject != gameObject);
        oscillators = GetComponentsInChildren<Oscillator>(true);

        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = navMeshAgent.angularSpeed;

        controller.enabled = false;
        shouldChange = true;

        GolemSlither_Input input = new(stateMachine, this);
        stateMachine.Init(input, new State_Follow());
        stateMachine.StateInput.SetPlayer(player);
    }

    private void GolemSlither_OnTimeScaleSet(float timeScale) {
        foreach (Oscillator oscillator in oscillators) {
            oscillator.SetTimeScale(timeScale);
            navMeshAgent.speed = baseLinearSpeed * timeScale;
            navMeshAgent.angularSpeed = baseAngularSpeed * timeScale;
        }
    }

    override protected void Update() {
        base.Update();
        stateMachine.Update();
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
        foreach (Oscillator osc in oscillators) osc.enabled = false;
        foreach (Rigidbody rb in rigidbodies) {
            rb.isKinematic = false;
            Vector3 force = new Vector3(UnityEngine.Random.Range(-0.15f, 0.15f), 0.85f, UnityEngine.Random.Range(-0.15f, 0.15f)) * UnityEngine.Random.Range(250, 300);
            rb.AddForce(force);
            Vector3 torque = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f)) * UnityEngine.Random.Range(250, 300);
            rb.AddTorque(torque);
        }

        DetachModules();

        Destroy(sweepRadius);
        Destroy(aggroRadius);
        Destroy(gameObject, 2);
    }

    public override void Perish() {
        base.Perish();
        Ragdoll();
    }

    private void Sweep() {
        StartCoroutine(ISweep());
    }

    private void Zig(Vector3 newPosition1, Vector3 newPosition2, Vector3 newPosition3) {
        StartCoroutine(IZig(newPosition1, newPosition2, newPosition3));
    }

    private void Wait() {
        StartCoroutine(IWait());
    }
    #endregion

    #region Coroutines
    private IEnumerator ISweep() {
        shouldChange = false;

        yield return new WaitForSeconds(0.4f / TimeScale);

        GolemSlither_SweepHitbox sweep = Instantiate(sweepHitbox, sweepPoint.position, sweepPoint.rotation);
        sweep.timeScale = TimeScale;

        yield return new WaitForSeconds(sweepDuration / TimeScale);

        didSweep = true;
        stateMachine.SetState(new State_Idle());
    }

    private IEnumerator IZig(Vector3 newPosition1, Vector3 newPosition2, Vector3 newPosition3) {
        shouldChange = false;
        Vector3 yOffset = new(0, sweepPoint.transform.position.y, 0);

        Vector3 originalPosition = controller.transform.position;
        controller.transform.position = newPosition1;

        GolemSlither_ZigHitbox zig1 = Instantiate(zigHitbox, (originalPosition + newPosition1) / 2.0f + yOffset, Quaternion.LookRotation(newPosition1 - originalPosition));
        zig1.transform.localScale = new Vector3(zig1.transform.localScale.x, zig1.transform.localScale.y, (newPosition1 - originalPosition).magnitude);

        yield return new WaitForSeconds(0.15f / TimeScale);

        controller.transform.position = newPosition2;

        GolemSlither_ZigHitbox zig2 = Instantiate(zigHitbox, (newPosition1 + newPosition2) / 2.0f + yOffset, Quaternion.LookRotation(newPosition2 - newPosition1));
        zig2.transform.localScale = new Vector3(zig2.transform.localScale.x, zig2.transform.localScale.y, (newPosition2 - newPosition1).magnitude);

        yield return new WaitForSeconds(0.15f / TimeScale);

        controller.transform.position = newPosition3;

        GolemSlither_ZigHitbox zig3 = Instantiate(zigHitbox, (newPosition2 + newPosition3) / 2.0f + yOffset, Quaternion.LookRotation(newPosition3 - newPosition2));
        zig3.transform.localScale = new Vector3(zig3.transform.localScale.x, zig3.transform.localScale.y, (newPosition3 - newPosition2).magnitude);

        yield return new WaitForSeconds(0.7f / TimeScale);

        if (zig1 != null) zig1.Detonate();
        if (zig2 != null) zig2.Detonate();
        if (zig3 != null) zig3.Detonate();

        shouldChange = true;
        didSweep = false;
        stateMachine.SetState(new State_Idle());
    }

    private IEnumerator IWait() {
        yield return new WaitForSeconds(didSweep ? sweepCooldownTime : zigCooldownTime);

        shouldChange = true;

        DecideAggro();
    }
    #endregion
}