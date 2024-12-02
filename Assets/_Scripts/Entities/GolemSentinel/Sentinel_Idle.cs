using System.Collections.Generic;
using UnityEngine;

public partial class GolemSentinel {

    private const string IDLE_PARAM = "Idle";

    [Header("Idle/Roam State")]

    [SerializeField] private Vector2 roamWaitTimeRange;
    [SerializeField] private Vector2 roamDistanceRange;
    [SerializeField] private float maxRoamDuration;
    [SerializeField] private float roamWallBuffer, roamSpeed;

    private class State_Idle : State<Sentinel_Input> {

        private float waitTimer, waitDuration;

        public override void Enter(Sentinel_Input input) {
            input.golem.MotionDriver.Set(input.golem.navMeshAgent);
            Vector2 waitRange = input.golem.roamWaitTimeRange;
            waitDuration = Random.Range(waitRange.x, waitRange.y);
            input.golem.animator.SetTrigger(IDLE_PARAM);
        }

        public override void Update(Sentinel_Input input) {
            waitTimer += input.golem.DeltaTime;
            if (waitTimer >= waitDuration) {
                input.stateMachine.SetState(new State_Roam());
            }
        }

        public override void Exit(Sentinel_Input input) { }
    }

    private class State_Roam : State<Sentinel_Input> {

        private Vector3 targetLocation;
        private float timer;

        public override void Enter(Sentinel_Input input) {
            GolemSentinel golem = input.golem;

            golem.BaseLinearSpeed = golem.roamSpeed;
            Vector2 distanceRange = golem.roamDistanceRange;
            float distance = Random.Range(distanceRange.x, distanceRange.y);

            if (PathfindingUtils.FindRandomRoamingPoint(golem.transform.position, distance, 
                                                        10, out targetLocation)) {
                golem.navMeshAgent.SetDestination(targetLocation);
            } else {
                input.stateMachine.SetState(new State_Idle());
            }
        }

        public override void Update(Sentinel_Input input) {
            GolemSentinel golem = input.golem;
            timer += golem.DeltaTime;
            if (golem.navMeshAgent.remainingDistance <= golem.navMeshAgent.stoppingDistance
                || timer >= golem.maxRoamDuration) {
                input.stateMachine.SetState(new State_Idle());
                input.golem.navMeshAgent.ResetPath();
            }
        }

        public override void Exit(Sentinel_Input input) { }
    }
}