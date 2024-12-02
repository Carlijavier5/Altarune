using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSavage {

    [Header("Ascension")]
    [SerializeField] private float ascensionTime;

    private class State_Ascension : State<Savage_Input> {

        private float timer;

        public override void Enter(Savage_Input input) {
            timer = input.savage.ascensionTime;
        }

        public override void Update(Savage_Input input) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                GolemSavage gs = input.savage;
                gs.DoPhaseTransition();
                if (gs.stagingPhase == SavagePhase.Phase2) {
                    input.microMachine.SetState(new State_EarthSpin());
                } else {
                    float attackTime = gs.activeConfig.RandomAttackTime;
                    input.microMachine.SetState(new State_Idle(gs.stagingPhase, attackTime));
                }
            }
        }

        public override void Exit(Savage_Input input) { }
    }
}