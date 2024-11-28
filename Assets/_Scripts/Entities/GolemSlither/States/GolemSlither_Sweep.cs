using UnityEngine;
using UnityEngine.AI;

public partial class GolemSlither {

    public class State_Chase : State<Slither_Input> {

        private NavMeshAgent agent;

        public override void Enter(Slither_Input input) {
            agent = input.golemSlither.navMeshAgent;
        }

        public override void Update(Slither_Input input) {
            agent.speed = input.golemSlither.chaseSpeed
                        * input.golemSlither.RootMult
                        * input.golemSlither.TimeScale;
            agent.SetDestination(input.aggroTarget.transform.position);
        }

        public override void Exit(Slither_Input input) {
            agent.ResetPath();
        }
    }

    public class State_Sweep : State<Slither_Input> {

        private Quaternion lookRotation;

        public override void Enter(Slither_Input input) {
            if (input.aggroTarget) {
                Vector3 dir = input.aggroTarget.transform.position
                            - input.golemSlither.transform.position;
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
                input.golemSlither.DeltaTime * input.golemSlither.navMeshAgent.angularSpeed);
        }

        public override void Exit(Slither_Input input) { }
    }
}