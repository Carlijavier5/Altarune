using UnityEngine;

public partial class GolemSiftling {

    private readonly int idleParam = Animator.StringToHash("Idle");
    private int ActiveIdleParam {
        get {
            return activeConfig.type switch {
                SiftlingType.Wind => fallParam,
                _ => idleParam,
            };
        }
    }

    [Header("Idle/Roam State")]

    [SerializeField] private float maxRoamDuration;
    [SerializeField] private float roamWallBuffer;

    private class State_IdleBase : State<Siftling_Input> {
        public override void Enter(Siftling_Input input) { }

        public override void Update(Siftling_Input input) {
            if (Time.time > input.siftling.canAttackTime
                    && input.aggroTarget != null) {
                State<Siftling_Input> attackState;
                switch (input.siftling.activeConfig.type) {
                    case SiftlingType.Normal:
                        attackState = new State_Precharge(input.aggroTarget);
                        input.stateMachine.SetState(new State_LookTarget(input.aggroTarget, attackState));
                        break;
                    case SiftlingType.Fire:
                        attackState = new State_FireWindUp();
                        input.stateMachine.SetState(new State_LookTarget(input.aggroTarget, attackState));
                        break;
                }
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
            input.siftling.animatorBody.enabled = false;

            input.siftling.animatorMain.ResetTrigger(input.siftling.preChargeParam);
            input.siftling.animatorMain.SetTrigger(input.siftling.ActiveIdleParam);

            input.siftling.oscillator.enabled = input.siftling.activeConfig.type == SiftlingType.Wind;
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

            input.siftling.animatorMain.SetTrigger(input.siftling.ActiveIdleParam);
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
