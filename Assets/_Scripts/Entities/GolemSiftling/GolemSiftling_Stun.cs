using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSiftling {

    private class State_Stun : State<Siftling_Input> {

        public override void Enter(Siftling_Input input) {
            GolemSiftling gs= input.siftling;
            gs.navMeshAgent.ResetPath();
            gs.navMeshAgent.enabled = false;
        }

        public override void Update(Siftling_Input _) { }

        public override void Exit(Siftling_Input input) {
            input.siftling.navMeshAgent.enabled = true;
        }
    }
}