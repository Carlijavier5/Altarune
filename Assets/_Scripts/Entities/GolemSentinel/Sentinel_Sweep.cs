using UnityEngine;

public partial class GolemSentinel {

    [Header("Sentinel Sweep")]
    [SerializeField] private GolemSweep sentinelSweep;

    private class State_Sweep : State_Aggro {

        private Quaternion lookRotation;

        public override void Enter(Sentinel_Input input) {
            if (input.aggroTarget) {
                input.golem.navMeshAgent.ResetPath();
                input.golem.navMeshAgent.enabled = false;
                input.golem.navMeshAgent.enabled = true;
                Vector3 dir = input.aggroTarget.transform.position
                            - input.golem.transform.position;
                dir.y = input.golem.transform.position.y;
                lookRotation = Quaternion.LookRotation(dir, Vector3.up);

                input.golem.sentinelSweep
                     .DoSweep(input.golem, lookRotation);
            } else {
                input.golem.UpdateAggro();
            }
        }

        public override void Update(Sentinel_Input input) {
            Transform t = input.golem.transform;
            t.rotation = Quaternion.RotateTowards(t.rotation, lookRotation,
                input.golem.DeltaTime * input.golem.navMeshAgent.angularSpeed * 5);
        }

        public override void Exit(Sentinel_Input input) {
            input.golem.animator.speed = input.golem.TimeScale;
        }
    }
}