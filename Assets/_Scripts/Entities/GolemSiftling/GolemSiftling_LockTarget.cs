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

    private class State_LockTarget : State_Aggro {

        public State_LockTarget(Entity aggroTarget) : base(aggroTarget) { }

        public override void Enter(Siftling_Input input) {
            base.Enter(input);

            input.siftling.exclamationVFX.Play();
            input.siftling.animatorMain.SetTrigger(IK_LOOK_PARAM);
            input.siftling.lookIKController.Play(input.aggroTarget.transform);

            input.siftling.navMeshAgent.updateRotation = false;
            input.siftling.navMeshAgent.ResetPath();
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
}
