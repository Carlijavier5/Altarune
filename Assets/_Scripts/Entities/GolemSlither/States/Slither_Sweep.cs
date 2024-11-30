using UnityEngine;
using UnityEngine.AI;

public partial class GolemSlither {

    [Header("Sweep Attack")]
    [SerializeField] private GolemSweep slitherSweep;
    [SerializeField] private float chaseDuration, chaseSpeed = 3f;

    public class State_Chase : State<Slither_Input> {

        private NavMeshAgent agent;
        private float timer;

        public override void Enter(Slither_Input input) {
            GolemSlither gs = input.golemSlither;
            gs.BaseLinearSpeed = gs.chaseSpeed;
            agent = gs.navMeshAgent;
            timer = gs.chaseDuration;
        }

        public override void Update(Slither_Input input) {
            agent.SetDestination(input.aggroTarget.transform.position);

            timer -= input.golemSlither.DeltaTime;
            if (timer <= 0) input.stateMachine.SetState(new State_Follow());
        }

        public override void Exit(Slither_Input input) {
            agent.ResetPath();
        }
    }

    public class State_Sweep : State<Slither_Input> {

        private Quaternion lookRotation;

        public override void Enter(Slither_Input input) {
            if (input.aggroTarget) {
                input.golemSlither.navMeshAgent.ResetPath();
                input.golemSlither.navMeshAgent.enabled = false;
                input.golemSlither.navMeshAgent.enabled = true;
                Vector3 dir = input.aggroTarget.transform.position
                            - input.golemSlither.transform.position;
                dir.y = input.golemSlither.transform.position.y;
                lookRotation = Quaternion.LookRotation(dir, Vector3.up);

                input.golemSlither.slitherSweep
                     .DoSweep(input.golemSlither, lookRotation);
            } else {
                input.golemSlither.UpdateAggro();
            }
        }

        public override void Update(Slither_Input input) {
            Transform t = input.golemSlither.transform;
            t.rotation = Quaternion.RotateTowards(t.rotation, lookRotation, 
                input.golemSlither.DeltaTime * input.golemSlither.navMeshAgent.angularSpeed * 5);
        }

        public override void Exit(Slither_Input input) {
            input.golemSlither.animator.speed = input.golemSlither.TimeScale;
        }
    }
}