using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSentinel {

    [Header("Stunned State")]

    [SerializeField] private Animator animator;
    private const string STUN_PARAMETER = "Stunned";

    private class State_Stunned : State<Sentinel_Input> {

        public override void Enter(Sentinel_Input input) {
            GolemSentinel golem = input.golem;

            if (golem.MotionDriver.MotionMode == MotionMode.NavMesh) {
                golem.navMeshAgent.ResetPath();
            }

            golem.animator.SetBool(STUN_PARAMETER, input.golem.IsStunned);
        }

        public override void Update(Sentinel_Input input) { }

        public override void Exit(Sentinel_Input input) {
            GolemSentinel golem = input.golem;

            golem.animator.SetBool(STUN_PARAMETER, input.golem.IsStunned);
        }
    }
}