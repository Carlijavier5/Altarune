using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSiftling {

    private const string IDLE_PARAM = "Idle";

    [Header("Idle/Roam State")]

    [SerializeField] private float maxRoamDuration;
    [SerializeField] private float roamWallBuffer;

    private class State_Idle : State<Siftling_Input> {

        private float waitTimer, waitDuration;

        public override void Enter(Siftling_Input input) {
            input.siftling.MotionDriver.Set(input.siftling.navMeshAgent);
            Vector2 waitRange = input.siftling.activeConfig.waitRange;
            waitDuration = Random.Range(waitRange.x, waitRange.y);
            input.siftling.BaseAnimatorSpeed = input.siftling.activeConfig.animationSpeed;
            input.siftling.animator.SetTrigger(IDLE_PARAM);
        }

        public override void Update(Siftling_Input input) {
            waitTimer += input.siftling.DeltaTime;
            if (waitTimer >= waitDuration) {
                input.stateMachine.SetState(new State_Roam());
            }
        }

        public override void Exit(Siftling_Input input) { }
    }

    private class State_Roam : State<Siftling_Input> {

        private Vector3 targetLocation;
        private float timer;

        public override void Enter(Siftling_Input input) {
            GolemSiftling golem = input.siftling;
            golem.navMeshAgent.enabled = true;
            golem.BaseLinearSpeed = golem.activeConfig.roamSpeed;
            Vector2 distanceRange = golem.activeConfig.distanceRange;
            float distance = Random.Range(distanceRange.x, distanceRange.y);

            if (PathfindingUtils.FindRandomRoamingPoint(golem.transform.position, distance,
                                                        10, out targetLocation)) {
                golem.navMeshAgent.SetDestination(targetLocation);
            } else {
                input.stateMachine.SetState(new State_Idle());
            }
        }

        public override void Update(Siftling_Input input) {
            GolemSiftling golem = input.siftling;
            timer += golem.DeltaTime;
            if (golem.navMeshAgent.isOnNavMesh
                && golem.navMeshAgent.remainingDistance <= golem.navMeshAgent.stoppingDistance
                    || timer >= golem.maxRoamDuration) {
                input.stateMachine.SetState(new State_Idle());
                input.siftling.navMeshAgent.ResetPath();
            }
        }

        public override void Exit(Siftling_Input input) { }
    }
}
