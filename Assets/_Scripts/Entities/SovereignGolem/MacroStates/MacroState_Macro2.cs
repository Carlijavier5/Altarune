using UnityEngine;

public partial class SovereignGolem {
    public class MacroState_Macro2 : MacroState {

        public override SovereignPhase Phase => SovereignPhase.Macro2;
        private SovereignAttack lastAttack = SovereignAttack.StaticLaser;
        private SovereignAttack lastLaser = SovereignAttack.StaticLaser;

        public override void Enter(Sovereign_Input input) {
            base.Enter(input);
            SovereignGolem sg = input.sovereign;
            sg.staticLaserMaster.EnterPhase(Phase);
            sg.swipingLaserMaster.EnterPhase(Phase);
            sg.pawSlamMaster.EnterPhase(Phase);
        }

        public override void PickAttack(Sovereign_Input input) {
            if (lastAttack == SovereignAttack.StaticLaser) {
                input.microMachine.SetState(new State_PawSlam(Phase));
                lastAttack = SovereignAttack.PawSlam;
            } else {
                if (lastLaser == SovereignAttack.StaticLaser) {
                    input.microMachine.SetState(new State_SwipingLaser(Phase));
                    lastLaser = SovereignAttack.SwipingLaser;
                } else {
                    input.microMachine.SetState(new State_StaticLaser(Phase));
                    lastLaser = SovereignAttack.StaticLaser;
                } lastAttack = SovereignAttack.StaticLaser;
            }
        }

        public override void Exit(Sovereign_Input input) { }
    }
}