using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSiftling
{
    [Header("Wind Descent State")]
    [SerializeField] private SiftlingBounceController bounceController;
    [SerializeField] private float pushPhaseStrength;
    [SerializeField] private float bounceDuration;

    private class State_WindDescent : State<Siftling_Input> {

        private float bounceEndTime;

        public override void Enter(Siftling_Input input) {
            input.siftling.activeConfig.tornado.Toggle(false);

            input.siftling.animatorMain.SetTrigger(FALL_PARAM);
            input.siftling.animatorBody.enabled = false;
            input.siftling.oscillator.enabled = false;

            input.siftling.bounceController.Play();
            bounceEndTime = Time.time + input.siftling.bounceDuration;
        }

        public override void Update(Siftling_Input input) {
            if (Time.time >= bounceEndTime) {
                input.siftling.animatorBody.enabled = true;
                input.siftling.stateMachine.SetState(new State_Ascend(false));
            }
        }

        public override void Exit(Siftling_Input input) {
            input.siftling.animatorBody.enabled = true;
            input.siftling.bounceController.Stop();
        }
    }
}