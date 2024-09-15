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
            Debug.Log("zigzag");
            _agent = input.shinobi.navMeshAgent;

            _agent.enabled = false;
            input.shinobi.controller.enabled = true;

            ZigZag(input);
        }

        public override void Update(Shinobi_Input input) {   }

        public override void Exit(Shinobi_Input input) 
        {
            input.shinobi.controller.enabled = false;
            _agent.enabled = true;
        }

        private void ZigZag(Shinobi_Input input)
        {
            if (input.shinobi.controller)
            {
                Vector3 playerPosition = input.player.transform.position;
                Vector3 directionToPlayer = (input.shinobi.controller.transform.position - playerPosition).normalized;

                Vector3 position1 = (playerPosition + directionToPlayer * -3f) / 3;
                Vector3 position2 = 2 * (playerPosition + directionToPlayer * -3f) / 3;
                Vector3 position3 = (playerPosition + directionToPlayer * -3f);

                input.shinobi.Zig(position1, position2, position3);
            }

            input.shinobi.stateMachine.SetState(new State_Idle());
        }
    }

    private class State_ChargingZigZag : State<Shinobi_Input>
    {

        private NavMeshAgent _agent;
        private float chargeTimer;
        private float chargeTime = 0.75f;
        private float chargeAmplitude = 0.2f;
        private Vector3 positionAnchor;

        public override void Enter(Shinobi_Input input)
        {
            Debug.Log("charging");
            _agent = input.shinobi.navMeshAgent;
            positionAnchor = input.shinobi.transform.position;

            _agent.ResetPath();
        }

        public override void Update(Shinobi_Input input)
        {
            chargeTimer = Mathf.MoveTowards(chargeTimer, chargeTime, Time.deltaTime);
            float chargePercent = chargeTimer / chargeTime;
            Transform t = input.shinobi.transform;
            t.position = new Vector3(positionAnchor.x + Random.Range(-chargeAmplitude,
                                                        chargeAmplitude) * chargePercent,
                                     positionAnchor.y,
                                     positionAnchor.z + Random.Range(-chargeAmplitude,
                                                        chargeAmplitude) * chargePercent);
            if (chargePercent >= 1) input.stateMachine.SetState(new State_ZigZag());
        }

        public override void Exit(Shinobi_Input input) { }
    }
}
