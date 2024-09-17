using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;

public partial class Shinobi : Entity
{
   public override float TimeScale
    {
        get => base.TimeScale;
        set
        {
            timeScale = value;
            foreach (Oscillator oscillator in oscillators)
            {
                oscillator.SetTimeScale(timeScale);
                navMeshAgent.speed = baseLinearSpeed * timeScale;
                navMeshAgent.angularSpeed = baseAngularSpeed * timeScale;
            }
        }
    }

    [NonSerialized] public readonly StateMachine<Shinobi_Input> stateMachine = new();

    [Header("Setup")]
    [SerializeField] private Shinobi_SweepRadius sweepRadius;
    [SerializeField] private Shinobi_AggroRadius aggroRadius;
    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform sweepPoint;
    [SerializeField] private Shinobi_SweepHitbox sweepHitbox;

    private IEnumerable<Rigidbody> rigidbodies;
    private IEnumerable<Oscillator> oscillators;
    private float baseLinearSpeed, baseAngularSpeed;

    private Entity player;

    [Header("Attributes")]
    [SerializeField] private float chaseDistance = 7.75f;
    [SerializeField] private float sweepDistance = 2.5f;
    [SerializeField] private float sweepDuration = 1.5f;
    [SerializeField] private float followSpeed = 0.75f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float cooldownTime = 1f;

    [NonSerialized] public bool shouldChange;

    private void Awake()
    {
        sweepRadius.GetComponent<SphereCollider>().radius = sweepDistance;
        aggroRadius.GetComponent<SphereCollider>().radius = chaseDistance;

        player = FindAnyObjectByType<Player>();

        rigidbodies = GetComponentsInChildren<Rigidbody>(true).Where((rb) => rb.gameObject != gameObject);
        oscillators = GetComponentsInChildren<Oscillator>(true);

        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = navMeshAgent.angularSpeed;

        controller.enabled = false;
        shouldChange = true;

        Shinobi_Input input = new(stateMachine, this);
        stateMachine.Init(input, new State_Follow());
        stateMachine.StateInput.SetPlayer(player);
    }

    override protected void Update()
    {
        base.Update();
        stateMachine.Update();
    }

    #region Behavior
    public void Aggro()
    {
        if (stateMachine.State == new State_ChargingZigZag())
        {
            return;
        }

        int rand = UnityEngine.Random.Range(0, 2);
        if (rand == 1)
        {
            stateMachine.SetState(new State_Chase());
        }
        else
        {
            stateMachine.SetState(new State_ChargingZigZag());
        }
    }

    private void DecideAggro()
    {
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) <= chaseDistance)
        {
            Aggro();
        }
        else
        {
            stateMachine.SetState(new State_Follow());
        }
    }
    public void Ragdoll()
    {
        foreach (Oscillator osc in oscillators) osc.enabled = false;
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
            Vector3 force = new Vector3(UnityEngine.Random.Range(-0.15f, 0.15f), 0.85f, UnityEngine.Random.Range(-0.15f, 0.15f)) * UnityEngine.Random.Range(250, 300);
            rb.AddForce(force);
            Vector3 torque = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f)) * UnityEngine.Random.Range(250, 300);
            rb.AddTorque(torque);
        }
        DetachModules();
        Destroy(this);
    }

    public override void Perish()
    {
        base.Perish();
        Ragdoll();
    }

    private void Sweep()
    {
        StartCoroutine(ISweep());
    }

    private void Zig(Vector3 newPosition1, Vector3 newPosition2, Vector3 newPosition3)
    {
        StartCoroutine(IZig(newPosition1, newPosition2, newPosition3));
    }

    private void Wait()
    {
        StartCoroutine(IWait());
    }
    #endregion

    #region Coroutines
    private IEnumerator ISweep()
    {
        shouldChange = false;

        yield return new WaitForSeconds(0.4f / TimeScale);

        Instantiate(sweepHitbox, sweepPoint.position, sweepPoint.rotation);

        yield return new WaitForSeconds(sweepDuration / TimeScale);

        stateMachine.SetState(new State_Idle());
    }

    private IEnumerator IZig(Vector3 newPosition1, Vector3 newPosition2, Vector3 newPosition3)
    {
        controller.transform.position = newPosition1;
        yield return new WaitForSeconds(0.15f / TimeScale);
        controller.transform.position = newPosition2;
        yield return new WaitForSeconds(0.15f / TimeScale);
        controller.transform.position = newPosition3;

        stateMachine.SetState(new State_Idle());
    }

    private IEnumerator IWait()
    {
        yield return new WaitForSeconds(cooldownTime);

        shouldChange = true;

        DecideAggro();
    }
    #endregion
}