using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    [SerializeField] private NavMeshAgent nma;
    [SerializeField] private Transform target;

    void Update() {
        nma.SetDestination(target.position);
    }
}
