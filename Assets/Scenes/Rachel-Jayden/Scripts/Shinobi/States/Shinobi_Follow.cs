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
            Debug.Log("follow");
            _agent = input.shinobi.navMeshAgent;
            _agent.speed = input.shinobi.followSpeed;
        }

        public override void Update(Shinobi_Input input)
        {
            _agent.SetDestination(input.player.transform.position);
        }

        public override void Exit(Shinobi_Input input)
        {
            _agent.ResetPath();
        }
    }
}
