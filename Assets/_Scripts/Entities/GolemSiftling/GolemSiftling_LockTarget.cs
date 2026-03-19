using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSiftling
{
    private readonly int ikLookParam = Animator.StringToHash("IKLook");

    [Header("Alert States")]
    [SerializeField] private SiftlingLookIKController lookIKController;
    [SerializeField] private SiftlingFaceTargetController lockFacingController;

    private class State_LookTarget : State_Aggro {

        private readonly State<Siftling_Input> followUpState;

        public State_LookTarget(Entity aggroTarget, State<Siftling_Input> followUpState) : base(aggroTarget) {
            this.followUpState = followUpState;
        }

        private float endLookTime;

        public override void Enter(Siftling_Input input) {
            base.Enter(input);

            input.siftling.exclamationVFX.Play();
            input.siftling.animatorMain.SetTrigger(input.siftling.ikLookParam);
            input.siftling.lookIKController.Play(input.aggroTarget.transform);

            input.siftling.navMeshAgent.updateRotation = false;
            input.siftling.navMeshAgent.ResetPath();

            endLookTime = Time.time + input.siftling.activeConfig.attackLookDuration;
        }

        public override void Update(Siftling_Input input) {
            if (Time.time > endLookTime && aggroTarget) {
                input.stateMachine.SetState(new State_FaceTarget(aggroTarget, followUpState));
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

        private readonly State<Siftling_Input> followUpState;

        public State_FaceTarget(Entity aggroTarget, State<Siftling_Input> followUpState) : base(aggroTarget) {
            this.followUpState = followUpState;
        }

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
            input.siftling.animatorMain.SetFloat(input.siftling.directionParam, input.siftling.TornadoDirectionMultiplier);

            input.siftling.navMeshAgent.ResetPath();
            input.siftling.navMeshAgent.updateRotation = true;

            if (input.stateMachine.NextState != followUpState) {
                input.siftling.TogglePhaseAttackIndicators(false);
            }
        }

        public override void PropagateAggroExit(Entity entity) {
            if (entity == aggroTarget) {
                input.stateMachine.SetState(new State_Idle());
            }
        }

        private void LockFacingController_OnFacingEnd() {
            input.stateMachine.SetState(followUpState);
        }
    }
}
