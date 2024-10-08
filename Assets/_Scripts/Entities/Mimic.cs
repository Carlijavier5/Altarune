using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mimic : MonoBehaviour
{
    public GameObject Player;
    public Transform playerTransform;
    public float detectRadius = 5f;
    public float atkRadius = 1.5f;
    public float moveSpeed = 5f;

    private NavMeshAgent agent;
    private CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distToPlayer <= detectRadius) {
            agent.SetDestination(playerTransform.position);
        }
        else {
            agent.ResetPath();
        }
    }
    void Attack() {
        agent.enabled = false;
        controller.enabled = true;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity) && entity.Faction != EntityFaction.Hostile) {
            bool isDamageable = entity.TryDamage(5);
        }
    }
}
