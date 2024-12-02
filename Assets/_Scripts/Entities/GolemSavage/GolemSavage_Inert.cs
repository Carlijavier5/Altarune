using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSavage {

    [Header("Inert")]
    [SerializeField] private float initDelay;
    private bool actorInRange;

    private class State_Inert : State<Savage_Input> {

        public override void Enter(Savage_Input _) { }

        public override void Update(Savage_Input _) { }

        public override void Exit(Savage_Input _) { }
    }
}