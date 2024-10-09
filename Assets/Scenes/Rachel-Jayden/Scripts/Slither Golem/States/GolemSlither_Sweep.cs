using UnityEngine;
using UnityEngine.AI;

public partial class GolemSlither {
    public class State_Sweep : State<GolemSlither_Input> {
        private NavMeshAgent _agent;

        public override void Enter(GolemSlither_Input input) {
            _agent = input.golemSlither.navMeshAgent;
            _agent.ResetPath();

            input.golemSlither.Sweep();
        }

        public override void Update(GolemSlither_Input input) {
            Transform t = input.golemSlither.transform;
            Quaternion targetRotation = Quaternion.LookRotation(input.golemSlither.player.transform.position - t.position, Vector3.up);
            t.rotation = Quaternion.RotateTowards(t.rotation, targetRotation, input.golemSlither.DeltaTime * input.golemSlither.navMeshAgent.angularSpeed);
        }

        public override void Exit(GolemSlither_Input input) { }
    }

    public class State_Chase : State<GolemSlither_Input> {
        private NavMeshAgent _agent;

        public override void Enter(GolemSlither_Input input) {
            _agent = input.golemSlither.navMeshAgent;
        }

        public override void Update(GolemSlither_Input input) {
            _agent.speed = input.golemSlither.chaseSpeed * input.golemSlither.TimeScale;
            _agent.SetDestination(input.player.transform.position);

            if (input.golemSlither.sweepRadius.shouldSweep) {
                input.stateMachine.SetState(new State_Sweep());
            }
        }

        public override void Exit(GolemSlither_Input input) {
            _agent.ResetPath();
        }
    }
}
