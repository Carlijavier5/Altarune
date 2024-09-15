using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Shinobi
{
    public class State_Sweep : State<Shinobi_Input>
    {
        private NavMeshAgent _agent;

        public override void Enter(Shinobi_Input input)
        {
            _agent = input.shinobi.navMeshAgent;
            _agent.ResetPath();

            input.shinobi.Sweep();
        }

        public override void Update(Shinobi_Input input) { }

        public override void Exit(Shinobi_Input input) { }
    }

    public class State_Chase : State<Shinobi_Input>
    {
        private NavMeshAgent _agent;

        public override void Enter(Shinobi_Input input)
        {
            Debug.Log("chase");
            _agent = input.shinobi.navMeshAgent;
            _agent.speed = input.shinobi.chaseSpeed;
        }

        public override void Update(Shinobi_Input input)
        {
            _agent.SetDestination(input.player.transform.position);

            if (input.shinobi.sweepRadius.shouldSweep)
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
