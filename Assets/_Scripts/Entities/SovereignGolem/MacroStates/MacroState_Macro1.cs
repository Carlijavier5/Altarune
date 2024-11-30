using UnityEngine;

public partial class SovereignGolem {
    public class MacroState_Macro1 : MacroState {

        public override SovereignPhase Phase => SovereignPhase.Macro1;

        public override void Enter(Sovereign_Input input) {
            base.Enter(input);
            SovereignGolem sg = input.sovereign;
            sg.staticLaserMaster.EnterPhase(Phase);
        }

        public override void PickAttack(Sovereign_Input input) {
            input.microMachine.SetState(new State_StaticLaser(Phase));
        }
    }
}