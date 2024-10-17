using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public partial class Golem {

    [Header("Idle/Roam State")]

    [SerializeField] private Vector2 roamWaitTimeRange;
    [SerializeField] private Vector2 roamDistanceRange;
    [SerializeField] private float maxRoamDuration;
    [SerializeField] private float roamWallBuffer, roamSpeed;

    private class State_Idle : State<Golem_Input> {

        private float waitTimer, waitDuration;

        public override void Enter(Golem_Input input) {
            input.golem.MotionDriver.Set(input.golem.navMeshAgent);
            Vector2 waitRange = input.golem.roamWaitTimeRange;
            waitDuration = Random.Range(waitRange.x, waitRange.y);
        }

        public override void Update(Golem_Input input) {
            waitTimer += input.golem.DeltaTime;
            if (waitTimer >= waitDuration) {
                input.stateMachine.SetState(new State_Roam());
            }
        }

        public override void Exit(Golem_Input input) { }
    }

    private class State_Roam : State<Golem_Input> {

        private Vector3 targetLocation;
        private float timer;

        public override void Enter(Golem_Input input) {
            Golem golem = input.golem;
            Transform t = golem.transform;

            int depthLimit = 0;
            while (targetLocation == Vector3.zero && depthLimit < 10) {
                Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                Vector2 distanceRange = golem.roamDistanceRange;
                float distance = Random.Range(distanceRange.x, distanceRange.y);

                Ray ray = new(t.position, dir); 
                IEnumerable<RaycastHit> info = Physics.RaycastAll(ray, distance + golem.roamWallBuffer)
                                                      .Where((info) => !info.collider.isTrigger);
                if (info.Count() == 0) {
                    targetLocation = ray.GetPoint(distance);
                } depthLimit++;
            } if (depthLimit >= 10) input.stateMachine.SetState(new State_Idle());
            else golem.navMeshAgent.SetDestination(targetLocation);
        }

        public override void Update(Golem_Input input) {
            Golem golem = input.golem;
            timer += golem.DeltaTime;
            if (golem.navMeshAgent.remainingDistance <= golem.navMeshAgent.stoppingDistance
                || timer >= golem.maxRoamDuration) {
                input.stateMachine.SetState(new State_Idle());
                input.golem.navMeshAgent.ResetPath();
            }
        }

        public override void Exit(Golem_Input input) { }
    }
}