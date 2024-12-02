using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSavage {

    private const string PERISH_PARAM = "Perish";

    public class State_Perish : State<Savage_Input> {

        public override void Enter(Savage_Input input) {
            GolemSavage gs = input.savage;
            gs.animator.SetTrigger(PERISH_PARAM);
        }

        public override void Update(Savage_Input _) { }

        public override void Exit(Savage_Input _) { }
    }
}