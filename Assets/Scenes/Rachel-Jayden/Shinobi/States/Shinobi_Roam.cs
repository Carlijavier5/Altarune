using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public partial class Shinobi
{
    public class State_Roam : State<Shinobi_Input>
    {
        private float _roamTime = 0f;
        private float _nextRoamTime = 0f;
        private NavMeshAgent _agent;

        public override void Enter(Shinobi_Input input)
        {
            Debug.Log("roam");

            _agent = input.shinobi.navMeshAgent;

            SetRandomDestination();
        }

        public override void Update(Shinobi_Input input)
        {
            _roamTime += Time.deltaTime;

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (_roamTime > _nextRoamTime)
                {
                    SetRandomDestination();
                }
            }
        }

        public override void Exit(Shinobi_Input input)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }

        private void SetRandomDestination()
        {
            // Reset roam timer and set the next roam time between 2 and 4 seconds
            _roamTime = 0f;
            _nextRoamTime = Random.Range(2f, 4f);

            // Get a random point on the NavMesh within a certain radius
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += _agent.transform.position;

            // Check if the random point is valid on the NavMesh
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
        }
    }
}
