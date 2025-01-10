using UnityEngine;

public partial class GolemSlither {

    private const string IDLE_PARAM = "Idle";

    [Header("Idle/Roam State")]

    [SerializeField] private Vector2 roamWaitTimeRange;
    [SerializeField] private Vector2 roamDistanceRange;
    [SerializeField] private float maxRoamDuration;
    [SerializeField] private float roamWallBuffer, roamSpeed;

    private class State_Idle : State<Slither_Input> {

        private float waitTimer, waitDuration;

        public override void Enter(Slither_Input input) {
            Vector2 waitRange = input.golemSlither.roamWaitTimeRange;
            waitDuration = Random.Range(waitRange.x, waitRange.y);
            input.golemSlither.animator.SetTrigger(IDLE_PARAM);
        }

        public override void Update(Slither_Input input) {
            waitTimer += input.golemSlither.DeltaTime;
            if (waitTimer >= waitDuration) {
                input.stateMachine.SetState(new State_Roam());
            }
        }

        public override void Exit(Slither_Input input) { }
    }

    private class State_Roam : State<Slither_Input> {

        private Vector3 targetLocation;
        private float timer;

        public override void Enter(Slither_Input input) {
            GolemSlither gs = input.golemSlither;

            gs.BaseLinearSpeed = gs.roamSpeed;
            Vector2 distanceRange = gs.roamDistanceRange;
            float distance = Random.Range(distanceRange.x, distanceRange.y);

            if (gs.navMeshAgent.isActiveAndEnabled && gs.navMeshAgent.isOnNavMesh
                && PathfindingUtils.FindRandomRoamingPoint(gs.transform.position, distance,
                                                        10, out targetLocation)) {
                gs.navMeshAgent.SetDestination(targetLocation);
            } else {
                input.stateMachine.SetState(new State_Idle());
            }
        }

        public override void Update(Slither_Input input) {
            GolemSlither gs = input.golemSlither;
            timer += gs.DeltaTime;
            if (gs.navMeshAgent.remainingDistance <= gs.navMeshAgent.stoppingDistance
                || timer >= gs.maxRoamDuration) {
                input.golemSlither.UpdateAggro();
                input.golemSlither.navMeshAgent.ResetPath();
            }
        }

        public override void Exit(Slither_Input input) { }
    }
}