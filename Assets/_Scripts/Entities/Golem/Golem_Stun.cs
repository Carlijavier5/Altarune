using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Golem {

    [Header("Stunned State")]

    [SerializeField] private Animator animator;
    private const string STUN_PARAMETER = "Stunned";

    private class State_Stunned : State<Golem_Input> {

        public override void Enter(Golem_Input input) {
            Golem golem = input.golem;

            if (golem.MotionDriver.MotionMode == MotionMode.NavMesh) {
                golem.navMeshAgent.ResetPath();
            }

            foreach (Oscillator oscillator in golem.oscillators) {
                oscillator.enabled = false;
            }

            golem.animator.SetBool(STUN_PARAMETER, input.golem.IsStunned);
        }

        public override void Update(Golem_Input input) { }

        public override void Exit(Golem_Input input) {
            Golem golem = input.golem;

            foreach (Oscillator oscillator in golem.oscillators) {
                oscillator.enabled = true;
            }
            golem.animator.SetBool(STUN_PARAMETER, input.golem.IsStunned);
        }
    }
}