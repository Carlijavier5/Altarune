using System.Collections.Generic;
using UnityEngine;

public partial class GolemSiftling {

    private const string ASCEND_PARAM = "Ascend",
                         DESCEND_PARAM = "Descend",
                         DESCEND_WIND_PARAM = "DescendWind";

    [SerializeField] private AnimationClip ascendClip;
    [SerializeField] private Material ascendMaterial;

    private class State_Ascend : State<Siftling_Input> {

        public override void Enter(Siftling_Input input) {
            GolemSiftling gs = input.siftling;
            gs.ApplyMaterial(gs.ascendMaterial);
            gs.animator.SetTrigger(ASCEND_PARAM);
            gs.TryToggleIFrame(true);
        }

        public override void Update(Siftling_Input input) { }

        public override void Exit(Siftling_Input input) {
            input.siftling.TryToggleIFrame(false);
        }
    }
}