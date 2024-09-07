using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public partial class Shinobi : MonoBehaviour
{
    private readonly StateMachine<Shinobi_Input> stateMachine = new();

    [Header("Setup")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Shinobi_SweepRadius sweepRadius;
    [SerializeField] private Material flashMat;
    [SerializeField] private Entity player;

    [Header("Attributes")]
    [SerializeField] private float chaseDistance = 7.75f;
    [SerializeField] private int health = 3;
    [SerializeField] private float followSpeed = 0.75f;
    [SerializeField] private float chaseSpeed = 3f;

    private void Awake()
    {
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
            stateMachine.SetState(new State_Chase());
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
            Debug.Log("ouch");
            return;
        }

        Debug.Log("died");
        Destroy(gameObject);
    }

    private void Sweep()
    {
        StartCoroutine(ISweep());
    }

    private bool _sweeping = false;

    private IEnumerator ISweep()
    {
        _sweeping = true;
        Debug.Log("sweep");
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
}