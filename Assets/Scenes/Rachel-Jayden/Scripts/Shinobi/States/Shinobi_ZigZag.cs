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
                Vector3 shinobiPosition = input.shinobi.controller.transform.position;
                Vector3 playerPosition = input.player.transform.position;

                Vector3 distanceToPlayerExtended = (playerPosition - shinobiPosition) * 1.3f;

                Vector3 distance1 = shinobiPosition + distanceToPlayerExtended / 3.0f;
                Vector3 distance2 = shinobiPosition + (2.0f * distanceToPlayerExtended / 3.0f);
                Vector3 distance3 = shinobiPosition + (3.0f * distanceToPlayerExtended / 3.0f);

                Vector3 offset1 = new(distanceToPlayerExtended.z / 3.0f, 0, -distanceToPlayerExtended.x / 3.0f);
                Vector3 offset2 = new(-distanceToPlayerExtended.z / 3.0f, 0, distanceToPlayerExtended.x / 3.0f);
                Vector3 offset3 = new(distanceToPlayerExtended.z / 3.0f, 0, -distanceToPlayerExtended.x / 3.0f);

                Vector3 position1 = distance1 + offset1;
                Vector3 position2 = distance2 + offset2;
                Vector3 position3 = distance3 + offset3;

                input.shinobi.Zig(position1, position2, position3);
            }
        }
    }

    private class State_ChargingZigZag : State<Shinobi_Input>
    {

        private NavMeshAgent _agent;
        private float chargeTimer;
        private readonly float chargeTime = 0.75f;
        private readonly float chargeAmplitude = 0.2f;
        private Vector3 positionAnchor;

        public override void Enter(Shinobi_Input input)
        {
            _agent = input.shinobi.navMeshAgent;
            positionAnchor = input.shinobi.transform.position;

            input.shinobi.shouldChange = false;
            _agent.ResetPath();
        }

        public override void Update(Shinobi_Input input)
        {
            chargeTimer = Mathf.MoveTowards(chargeTimer, chargeTime, Time.deltaTime * input.shinobi.TimeScale);
            float chargePercent = chargeTimer / chargeTime;
            Transform t = input.shinobi.transform;
            t.position = new Vector3(positionAnchor.x + Random.Range(-chargeAmplitude,
                                                        chargeAmplitude) * chargePercent,
                                     positionAnchor.y,
                                     positionAnchor.z + Random.Range(-chargeAmplitude,
                                                        chargeAmplitude) * chargePercent);
            if (chargePercent >= 1)
            {
                input.stateMachine.SetState(new State_ZigZag());
            }
        }

        public override void Exit(Shinobi_Input input)
        {
            input.shinobi.shouldChange = true;
        }
    }
}
