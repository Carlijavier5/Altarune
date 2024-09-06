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
            _agent.speed = 0.75f;
        }

        public override void Update(Shinobi_Input input)
        {
            _agent.SetDestination(input.player.transform.position);

            if (input.shinobi.sweepRadius.ShouldSweep)
            {
                input.stateMachine.SetState(new State_Sweep());
            }
        }

        public override void Exit(Shinobi_Input input)
        {
            _agent.ResetPath();
        }
    }
}
