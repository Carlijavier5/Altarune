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

        public override void Update(Shinobi_Input input)
        {
            Transform t = input.shinobi.transform;
            Quaternion targetRotation = Quaternion.LookRotation(input.shinobi.player.transform.position - t.position, Vector3.up);
            t.rotation = Quaternion.RotateTowards(t.rotation, targetRotation, input.shinobi.DeltaTime * input.shinobi.navMeshAgent.angularSpeed);
        }

        public override void Exit(Shinobi_Input input) { }
    }

    public class State_Chase : State<Shinobi_Input>
    {
        private NavMeshAgent _agent;

        public override void Enter(Shinobi_Input input)
        {
            _agent = input.shinobi.navMeshAgent;
        }

        public override void Update(Shinobi_Input input)
        {
            _agent.speed = input.shinobi.chaseSpeed * input.shinobi.TimeScale;
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
