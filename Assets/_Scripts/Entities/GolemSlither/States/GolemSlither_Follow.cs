using UnityEngine.AI;

public partial class GolemSlither
{
    public class State_Follow : State<Slither_Input> {
        private NavMeshAgent agent;

        public override void Enter(Slither_Input input) {
            agent = input.golemSlither.navMeshAgent;
        }

        public override void Update(Slither_Input input) {
            agent.SetDestination(input.aggroTarget.transform.position);
            agent.speed = input.golemSlither.followSpeed
                        * input.golemSlither.RootMult
                        * input.golemSlither.TimeScale;

            if (agent.hasPath && !agent.pathPending && agent.remainingDistance < input.golemSlither.chaseDistance) {
                input.golemSlither.Aggro();
            }
        }

        public override void Exit(Slither_Input input) {
            agent.ResetPath();
        }
    }
}