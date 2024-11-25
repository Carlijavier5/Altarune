using UnityEngine;
using UnityEngine.AI;

public class PositionRandomizer : MonoBehaviour {

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float moveRadius, moveDuration;
    private float timer;

    void Update() {
        if (agent.enabled && (timer <= 0 || agent.remainingDistance <= agent.stoppingDistance)
            && PathfindingUtils.FindRandomRoamingPoint(transform.position, moveRadius,
                                                       10, out Vector3 clearPoint)
            && NavMesh.SamplePosition(clearPoint, out NavMeshHit hit, agent.height, NavMesh.AllAreas)) {
            agent.SetDestination(hit.position);
            timer = moveDuration;
        } timer -= Time.deltaTime;
    }

    public void Toggle(bool on) => agent.enabled = on;
}