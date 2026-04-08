using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSiftling
{
    [Header("Wind Rift")]
    [SerializeField] private SiftlingWindDriftIKController windDriftIKController;
    [SerializeField] private SiftlingWindFaceTargetController windFaceTargetController;

    private class State_WindLookTarget : State_LookTarget {

        private float startTime, startSpeed;

        public State_WindLookTarget(Entity aggroTarget, State<Siftling_Input> followUpState) : base(aggroTarget, followUpState) { }

        public override void Enter(Siftling_Input input) {
            base.Enter(input);

            startTime = Time.time;
            startSpeed = input.siftling.activeConfig.roamSpeed;

            input.siftling.navMeshAgent.autoBraking = false;
            input.siftling.navMeshAgent.updateRotation = true;
        }

        public override void Update(Siftling_Input input) {
            float lerpVal = (Time.time - startTime) / (endLookTime - startTime);
            input.siftling.navMeshAgent.speed = Mathf.Lerp(startSpeed, 0, lerpVal);

            if (aggroTarget) {
                input.siftling.navMeshAgent.SetDestination(aggroTarget.transform.position);
            }

            base.Update(input);
        }

        public override void Exit(Siftling_Input input) {
            base.Exit(input);
            input.siftling.navMeshAgent.autoBraking = true;
        }
    }

    private class State_WindFaceTarget : State_Aggro {

        public State_WindFaceTarget(Entity aggroTarget) : base(aggroTarget) { }

        public override void Enter(Siftling_Input input) {
            base.Enter(input);
            /// Set trigger to raise foot;
        }

        public override void Update(Siftling_Input input) {
            /// Lower animation speed and wait till foot is raised;
            /// Begin spinning in the direction specified;
            /// Halfway through the spin lock the direction to where the player is currently at and deploy indicators;
            /// Resume animation raising the speed such that the foot lands when the siftling reaches the bottom;
            /// Then, advance to the next state;
        }

        public override void Exit(Siftling_Input input) {
            base.Exit(input);
        }
    }
}
