using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSiftling
{
    private const string IK_LOOK_PARAM = "IKLook";

    [Header("Alert States")]
    [SerializeField] private SiftlingLookIKController lookIKController;
    [SerializeField] private SiftlingFaceTargetController lockFacingController;

    private class State_LookTarget : State_Aggro {

        public State_LookTarget(Entity aggroTarget) : base(aggroTarget) { }

        private float endLookTime;

        public override void Enter(Siftling_Input input) {
            base.Enter(input);

            input.siftling.exclamationVFX.Play();
            input.siftling.animatorMain.SetTrigger(IK_LOOK_PARAM);
            input.siftling.lookIKController.Play(input.aggroTarget.transform);

            input.siftling.navMeshAgent.updateRotation = false;
            input.siftling.navMeshAgent.ResetPath();

            endLookTime = Time.time + input.siftling.activeConfig.attackLookDuration;
        }

        public override void Update(Siftling_Input input) {
            if (Time.time > endLookTime && aggroTarget) {
                input.stateMachine.SetState(new State_FaceTarget(aggroTarget));
            }
        }

        public override void Exit(Siftling_Input input) {
            base.Exit(input);

            input.siftling.lookIKController.Stop();

            input.siftling.navMeshAgent.ResetPath();
            input.siftling.navMeshAgent.updateRotation = true;
        }

        public override void PropagateAggroExit(Entity entity) {
            if (entity == aggroTarget) {
                input.stateMachine.SetState(new State_Idle());
            }
        }
    }

    private class State_FaceTarget : State_Aggro {

        public State_FaceTarget(Entity aggroTarget) : base(aggroTarget) { }

        public override void Enter(Siftling_Input input) {
            base.Enter(input);

            if (input.aggroTarget) {
                input.siftling.navMeshAgent.updateRotation = false;
                input.siftling.navMeshAgent.ResetPath();

                input.siftling.lockFacingController.OnFacingEnd += LockFacingController_OnFacingEnd;
                input.siftling.lockFacingController.Play(input.aggroTarget.transform);
            }
        }

        public override void Update(Siftling_Input input) { }

        public override void Exit(Siftling_Input input) {
            base.Exit(input);

            input.siftling.lockFacingController.OnFacingEnd -= LockFacingController_OnFacingEnd;

            input.siftling.lockFacingController.Stop();
            input.siftling.animatorMain.SetFloat(DIRECTION_PARAM, input.siftling.TornadoDirectionMultiplier);

            input.siftling.navMeshAgent.ResetPath();
            input.siftling.navMeshAgent.updateRotation = true;
        }

        public override void PropagateAggroExit(Entity entity) {
            if (entity == aggroTarget) {
                input.stateMachine.SetState(new State_Idle());
            }
        }

        private void LockFacingController_OnFacingEnd() {
            input.stateMachine.SetState(new State_FireWindUp());
        }
    }
}
