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
            Debug.Log("sweep");
            _agent = input.shinobi.navMeshAgent;
            _agent.ResetPath();
        }

        public override void Update(Shinobi_Input input)
        {
            if (!input.shinobi._sweeping)
            {
                input.shinobi.Sweep();

                input.stateMachine.SetState(new State_Idle());
            }
        }

        public override void Exit(Shinobi_Input input) { }
    }
}
