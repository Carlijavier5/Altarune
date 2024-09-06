using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public partial class Shinobi : MonoBehaviour
{
    private readonly StateMachine<Shinobi_Input> stateMachine = new();

    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private int health = 3;

    private void Awake()
    {
        controller.enabled = false;

        Shinobi_Input input = new(stateMachine, this);
        stateMachine.Init(input, new State_Roam());
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            stateMachine.StateInput.SetTarget(player);
            stateMachine.SetState(new State_Aggro());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            stateMachine.StateInput.SetTarget(null);
            stateMachine.SetState(new State_Roam());
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
}