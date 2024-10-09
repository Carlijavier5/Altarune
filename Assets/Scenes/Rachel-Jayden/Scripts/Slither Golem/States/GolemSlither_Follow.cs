using UnityEngine.AI;

public partial class GolemSlither
{
    public class State_Follow : State<GolemSlither_Input> {
        private NavMeshAgent _agent;

        public override void Enter(GolemSlither_Input input) {
            _agent = input.golemSlither.navMeshAgent;
        }

        public override void Update(GolemSlither_Input input) {
            _agent.SetDestination(input.player.transform.position);
            _agent.speed = input.golemSlither.followSpeed * input.golemSlither.TimeScale;

            if (_agent.hasPath && !_agent.pathPending && _agent.remainingDistance < input.golemSlither.chaseDistance) {
                input.golemSlither.Aggro();
            }
        }

        public override void Exit(GolemSlither_Input input) {
            _agent.ResetPath();
        }
    }
}
