using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public partial class Shinobi
{
    public class State_ZigZag : State<Shinobi_Input>
    {
        private NavMeshAgent _agent;

        public override void Enter(Shinobi_Input input)
        {
            _agent = input.shinobi.navMeshAgent;

            _agent.enabled = false;
            input.shinobi.controller.enabled = true;
            Debug.Log("zig");
        }

        public override void Update(Shinobi_Input input)
        {
            if (input.shinobi.controller != null)
            {
                Vector3 playerPosition = input.player.transform.position;
                Vector3 directionToPlayer = (input.shinobi.controller.transform.position - playerPosition).normalized;
                Vector3 offsetPosition1 = (playerPosition + directionToPlayer * -3f)/3;
                Vector3 offsetPosition2 = 2 * (playerPosition + directionToPlayer * -3f) / 3;
                Vector3 offsetPosition3 = (playerPosition + directionToPlayer * -3f);

                input.shinobi.controller.transform.position = offsetPosition1;
                input.shinobi.Wait();
                input.shinobi.controller.transform.position = offsetPosition2;
                input.shinobi.Wait();
                input.shinobi.controller.transform.position = offsetPosition3;
            }

            if (input.shinobi.sweepRadius.shouldSweep)
            {
                input.stateMachine.SetState(new State_Follow());
            }
        }

        public override void Exit(Shinobi_Input input) 
        {
            input.shinobi.controller.enabled = false;
            _agent.enabled = true;
        }
    }
}
