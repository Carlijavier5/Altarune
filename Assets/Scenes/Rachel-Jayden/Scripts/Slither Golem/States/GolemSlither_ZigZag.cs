using UnityEngine;
using UnityEngine.AI;

public partial class GolemSlither {
    public class State_ZigZag : State<GolemSlither_Input> {
        private NavMeshAgent _agent;

        public override void Enter(GolemSlither_Input input) {
            _agent = input.golemSlither.navMeshAgent;

            _agent.enabled = false;
            input.golemSlither.controller.enabled = true;

            ZigZag(input);
        }

        public override void Update(GolemSlither_Input input) { }

        public override void Exit(GolemSlither_Input input) {
            input.golemSlither.controller.enabled = false;
            _agent.enabled = true;
        }

        private void ZigZag(GolemSlither_Input input) {
            if (input.golemSlither.controller) {
                Vector3 golemSlitherPosition = input.golemSlither.controller.transform.position;
                Vector3 playerPosition = input.player.transform.position;

                Vector3 distanceToPlayerExtended = (playerPosition - golemSlitherPosition) * 1.3f;

                Vector3 distance1 = golemSlitherPosition + distanceToPlayerExtended / 3.0f;
                Vector3 distance2 = golemSlitherPosition + (2.0f * distanceToPlayerExtended / 3.0f);
                Vector3 distance3 = golemSlitherPosition + (3.0f * distanceToPlayerExtended / 3.0f);

                Vector3 offset1 = new(distanceToPlayerExtended.z / 3.0f, 0, -distanceToPlayerExtended.x / 3.0f);
                Vector3 offset2 = new(-distanceToPlayerExtended.z / 3.0f, 0, distanceToPlayerExtended.x / 3.0f);
                Vector3 offset3 = new(distanceToPlayerExtended.z / 3.0f, 0, -distanceToPlayerExtended.x / 3.0f);

                Vector3 position1 = distance1 + offset1;
                Vector3 position2 = distance2 + offset2;
                Vector3 position3 = distance3 + offset3;

                input.golemSlither.Zig(position1, position2, position3);
            }
        }
    }

    private class State_ChargingZigZag : State<GolemSlither_Input> {

        private NavMeshAgent _agent;
        private float chargeTimer;
        private readonly float chargeTime = 0.75f;
        private readonly float chargeAmplitude = 0.2f;
        private Vector3 positionAnchor;

        public override void Enter(GolemSlither_Input input) {
            _agent = input.golemSlither.navMeshAgent;
            positionAnchor = input.golemSlither.transform.position;

            input.golemSlither.shouldChange = false;
            _agent.ResetPath();
        }

        public override void Update(GolemSlither_Input input) {
            Transform t = input.golemSlither.transform;
            Quaternion targetRotation = Quaternion.LookRotation(input.golemSlither.player.transform.position - t.position, Vector3.up);
            t.rotation = Quaternion.RotateTowards(t.rotation, targetRotation, input.golemSlither.DeltaTime * _agent.angularSpeed);

            chargeTimer = Mathf.MoveTowards(chargeTimer, chargeTime, Time.deltaTime * input.golemSlither.TimeScale);
            float chargePercent = chargeTimer / chargeTime;
            t.position = new Vector3(positionAnchor.x + Random.Range(-chargeAmplitude,
                                                        chargeAmplitude) * chargePercent,
                                     positionAnchor.y,
                                     positionAnchor.z + Random.Range(-chargeAmplitude,
                                                        chargeAmplitude) * chargePercent);
            if (chargePercent >= 1) {
                input.stateMachine.SetState(new State_ZigZag());
            }
        }

        public override void Exit(GolemSlither_Input input) {
            input.golemSlither.shouldChange = true;
        }
    }
}
