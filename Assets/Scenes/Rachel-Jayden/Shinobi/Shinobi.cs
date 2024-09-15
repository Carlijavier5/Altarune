using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public partial class Shinobi : MonoBehaviour
{
    private readonly StateMachine<Shinobi_Input> stateMachine = new();

    [Header("Setup")]
    [SerializeField] private Shinobi_SweepRadius sweepRadius;
    [SerializeField] private Material flashMat;
    [SerializeField] private Entity player;

    private CharacterController controller;
    private NavMeshAgent navMeshAgent;

    [Header("Attributes")]
    [SerializeField] private float chaseDistance = 7.75f;
    [SerializeField] private int health = 3;
    [SerializeField] private float followSpeed = 0.75f;
    [SerializeField] private float chaseSpeed = 3f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        controller.enabled = false;

        Shinobi_Input input = new(stateMachine, this);
        stateMachine.Init(input, new State_Follow());
        stateMachine.StateInput.SetPlayer(player);
        gameObject.GetComponent<SphereCollider>().radius = chaseDistance;
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            Aggro();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            stateMachine.SetState(new State_Follow());
        }
    }

    public void TakeDamage()
    {
        health--;

        if (health > 0)
        {
            return;
        }

        Destroy(gameObject);
    }

    private void Aggro()
    {
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

    private bool _sweeping = false;

    private IEnumerator ISweep()
    {
        _sweeping = true;
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        Dictionary<Renderer, Material[]> matDict = new();
        foreach (Renderer renderer in renderers)
        {
            matDict[renderer] = renderer.sharedMaterials;
            renderer.sharedMaterial = flashMat;
        }
        yield return new WaitForSeconds(2);
        foreach (KeyValuePair<Renderer, Material[]> kvp in matDict)
        {
            kvp.Key.sharedMaterials = kvp.Value;
        }
        _sweeping = false;
    }

    private IEnumerator IZig(Vector3 newPosition1, Vector3 newPosition2, Vector3 newPosition3)
    {
        controller.transform.position = newPosition1;
        yield return new WaitForSeconds(0.1f);
        controller.transform.position = newPosition2;
        yield return new WaitForSeconds(0.1f);
        controller.transform.position = newPosition3;
    }

    private IEnumerator IWait()
    {
        yield return new WaitForSeconds(1.5f);

        if (Vector3.Distance(player.transform.position, gameObject.transform.position) <= chaseDistance)
        {
            Aggro();
        }
        else
        {
            stateMachine.SetState(new State_Follow());
        }
    }
}