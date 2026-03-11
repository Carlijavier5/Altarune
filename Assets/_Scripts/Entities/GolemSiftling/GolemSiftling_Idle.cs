using UnityEngine;

public partial class GolemSiftling {

    private const string IDLE_PARAM = "Idle";

    [Header("Idle/Roam State")]

    [SerializeField] private float maxRoamDuration;
    [SerializeField] private float roamWallBuffer;

    private class State_IdleBase : State<Siftling_Input> {
        public override void Enter(Siftling_Input input) { }

        public override void Update(Siftling_Input input) {
            if (//Time.time > input.siftling.canAttackTime
                    /*&&*/ input.aggroTarget != null) {
                /*
                switch (input.siftling.activeConfig.type) {
                    case SiftlingType.Normal:
                        input.stateMachine.SetState(new State_Precharge(input.aggroTarget));
                        break;
                    case SiftlingType.Fire:
                        input.stateMachine.SetState(new State_FireCastAnticipation(input.aggroTarget));
                        break;
                }*/
                input.stateMachine.SetState(new State_LockTarget(input.aggroTarget));
            }
        }

        public override void Exit(Siftling_Input input) { }
    }

    private class State_Idle : State_IdleBase {

        private float waitTimer, waitDuration;

        public override void Enter(Siftling_Input input) {
            input.siftling.MotionDriver.Set(input.siftling.navMeshAgent);

            Vector2 waitRange = input.siftling.activeConfig.waitRange;
            waitDuration = Random.Range(waitRange.x, waitRange.y);

            input.siftling.BaseMainAnimatorSpeed = input.siftling.activeConfig.animationSpeed;

            input.siftling.animatorMain.ResetTrigger(PRE_CHARGE_PARAM);
            input.siftling.animatorMain.SetTrigger(IDLE_PARAM);

            switch(input.siftling.activeConfig.type) {
                case SiftlingType.Wind:
                    input.siftling.oscillator.enabled = true;
                    break;
            }
        }

        public override void Update(Siftling_Input input) {
            waitTimer += input.siftling.DeltaTime;
            if (waitTimer >= waitDuration) {
                input.stateMachine.SetState(new State_Roam());
            } else {
                base.Update(input);
            }
        }

        public override void Exit(Siftling_Input input) { }
    }

    private class State_Roam : State_IdleBase {

        private Vector3 targetLocation;
        private float endTime;

        public override void Enter(Siftling_Input input) {
            GolemSiftling golem = input.siftling;
            golem.BaseLinearSpeed = golem.activeConfig.roamSpeed;
            Vector2 distanceRange = golem.activeConfig.distanceRange;
            float distance = Random.Range(distanceRange.x, distanceRange.y);

            input.siftling.animatorMain.SetTrigger(IDLE_PARAM);
            endTime = Time.time + golem.maxRoamDuration;

            if (PathfindingUtils.FindRandomRoamingPoint(golem.transform.position, distance,
                                                        10, out targetLocation)) {
                golem.navMeshAgent.SetDestination(targetLocation);
            } else {
                input.stateMachine.SetState(new State_Idle());
            }
        }

        public override void Update(Siftling_Input input) {
            GolemSiftling golem = input.siftling;
            if (golem.navMeshAgent.isOnNavMesh
                    && golem.navMeshAgent.remainingDistance <= golem.navMeshAgent.stoppingDistance
                        || Time.time > endTime) {
                input.stateMachine.SetState(new State_Idle());
                input.siftling.navMeshAgent.ResetPath();
            } else {
                base.Update(input);
            }
        }

        public override void Exit(Siftling_Input input) {
            input.siftling.navMeshAgent.ResetPath();
        }
    }
}
