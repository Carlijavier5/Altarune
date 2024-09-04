using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mimic : MonoBehaviour
{
    public Transform player;
    public float detectRadius = 5f;
    public float moveSpeed = 3.5f;

    private NavMeshAgent agent;
    private CharacterController char;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        char = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (distToPlayer <= detectRadius) {
            agent.SetDestination(player.position);
        }
        else {
            agent.ResetPath();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("damage will be dealt");
        }
    }
}
