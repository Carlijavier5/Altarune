using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSiftling
{
    private const string IK_LOOK_PARAM = "IKLook",
                         RAISE_LEG_PARAM = "RaiseLeg",
                         JUMP_PARAM = "Jump";

    [Header("Alert States")]
    [SerializeField] private SiftlingLookIKController lookIKController;
    [SerializeField] private float maxDoFaceDuration;

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

            string param = GetFacingAnimationParam();
            input.siftling.animatorMain.SetTrigger(param);

            input.siftling.navMeshAgent.updateRotation = false;
            input.siftling.navMeshAgent.ResetPath();
        }

        public override void Update(Siftling_Input input) {
            /// Await animation trigger to rotate on a min/max scale;
            /// Snap at max duration as a fallback;
            /// Move siftling up/down if jump based on an animation curve and a max/min height;
            /// Look at canattack time in aggro state to see why it's not used;
        }

        public override void Exit(Siftling_Input input) {
            base.Exit(input);
            input.siftling.navMeshAgent.ResetPath();
            input.siftling.navMeshAgent.updateRotation = true;
        }

        public override void PropagateAggroExit(Entity entity) {
            if (entity == aggroTarget) {
                input.stateMachine.SetState(new State_Idle());
            }
        }

        private string GetFacingAnimationParam() {
            return ""; /// Check angle and decide whether to step or jump;
        }
    }
}
