using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public partial class Shinobi : Entity
{
    [NonSerialized] public readonly StateMachine<Shinobi_Input> stateMachine = new();

    [Header("Setup")]
    [SerializeField] private Shinobi_SweepRadius sweepRadius;
    [SerializeField] private Material flashMat;
    
    private Entity player;
    private CharacterController controller;
    private NavMeshAgent navMeshAgent;

    [Header("Attributes")]
    public float chaseDistance = 7.75f;
    [SerializeField] private float followSpeed = 0.75f;
    [SerializeField] private float chaseSpeed = 3f;

    [NonSerialized] public bool shouldChange;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = FindAnyObjectByType<Player>();

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
        Debug.Log("SWEEPING");

        shouldChange = false;

        yield return new WaitForSeconds(2);

        stateMachine.SetState(new State_Idle());
    }

    private IEnumerator IZig(Vector3 newPosition1, Vector3 newPosition2, Vector3 newPosition3)
    {
        controller.transform.position = newPosition1;
        yield return new WaitForSeconds(0.15f);
        controller.transform.position = newPosition2;
        yield return new WaitForSeconds(0.15f);
        controller.transform.position = newPosition3;

        stateMachine.SetState(new State_Idle());
    }

    private IEnumerator IWait()
    {
        yield return new WaitForSeconds(1.5f);

        shouldChange = true;

        DecideAggro();
    }
    #endregion
}