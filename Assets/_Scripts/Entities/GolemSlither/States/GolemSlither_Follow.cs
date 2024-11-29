using UnityEngine;
using UnityEngine.AI;

public partial class GolemSlither
{
    [Header("Follow State")]
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float followSpeed;
    [SerializeField] private Vector2 attackWaitRange;

    public class State_Follow : State<Slither_Input> {

        private NavMeshAgent agent;
        private float timer;

        public override void Enter(Slither_Input input) {
            GolemSlither gs = input.golemSlither;
            agent = gs.navMeshAgent;
            timer = Random.Range(gs.attackWaitRange.x, gs.attackWaitRange.y);
        }

        public override void Update(Slither_Input input) {
            if (input.aggroTarget) {
                agent.SetDestination(input.aggroTarget.transform.position);
                agent.speed = input.golemSlither.followSpeed
                            * input.golemSlither.RootMult
                            * input.golemSlither.TimeScale;

                timer = Mathf.MoveTowards(timer, 0, input.golemSlither.DeltaTime);
                if (agent.remainingDistance < input.golemSlither.attackDistance
                        && timer <= 0) {
                    input.golemSlither.TryAttack();
                    timer = Random.Range(input.golemSlither.attackWaitRange.x,
                                         input.golemSlither.attackWaitRange.y);
                }
            } else input.golemSlither.UpdateAggro();
        }

        public override void Exit(Slither_Input input) {
            agent.ResetPath();
        }
    }
}