using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public partial class Shinobi
{
    public class State_Follow : State<Shinobi_Input>
    {
        private NavMeshAgent _agent;

        public override void Enter(Shinobi_Input input)
        {
            _agent = input.shinobi.navMeshAgent;
        }

        public override void Update(Shinobi_Input input)
        {
            _agent.SetDestination(input.player.transform.position);
            _agent.speed = input.shinobi.followSpeed * input.shinobi.TimeScale;

            if (_agent.hasPath && !_agent.pathPending && _agent.remainingDistance < input.shinobi.chaseDistance)
            {
                Debug.Log(_agent.remainingDistance);
                Debug.Log(input.shinobi.chaseDistance);
                input.shinobi.Aggro();
            }
        }

        public override void Exit(Shinobi_Input input)
        {
            _agent.ResetPath();
        }
    }
}
