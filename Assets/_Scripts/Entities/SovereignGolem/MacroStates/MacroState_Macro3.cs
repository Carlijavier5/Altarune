using UnityEngine;

public partial class SovereignGolem {

    public class MacroState_Macro3 : MacroState {

        public override SovereignPhase Phase => SovereignPhase.Macro3;
        private readonly SovereignAttack[] attackPool = new[] { SovereignAttack.StaticLaser,
                                                                SovereignAttack.SwipingLaser,
                                                                SovereignAttack.PawSlam };

        public override void Enter(Sovereign_Input input) {
            base.Enter(input);
            SovereignGolem sg = input.sovereign;
            sg.staticLaserMaster.EnterPhase(Phase);
            sg.swipingLaserMaster.EnterPhase(Phase);
            sg.pawSlamMaster.EnterPhase(Phase);
        }

        public override void PickAttack(Sovereign_Input input) {
            int attackIndex = Random.Range(0, attackPool.Length);
            SovereignAttack attackType = attackPool[attackIndex];
            switch (attackType) {
                case SovereignAttack.StaticLaser:
                    input.microMachine.SetState(new State_StaticLaser(Phase));
                    break;
                case SovereignAttack.SwipingLaser:
                    input.microMachine.SetState(new State_SwipingLaser(Phase));
                    break;
                case SovereignAttack.PawSlam:
                    input.microMachine.SetState(new State_PawSlam(Phase));
                    break;
            }
        }
    }
}