using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSiftling {
    private class Siftling_Input : StateInput {

        public readonly StateMachine<Siftling_Input> stateMachine;
        public GolemSiftling siftling;

        public Siftling_Input(StateMachine<Siftling_Input> stateMachine,
                               GolemSiftling siftling) {
            this.stateMachine = stateMachine;
            this.siftling = siftling;
        }
    }
}