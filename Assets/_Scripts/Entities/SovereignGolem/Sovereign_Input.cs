public partial class SovereignGolem {
    public class Sovereign_Input : StateInput {

        public readonly StateMachine<Sovereign_Input> macroMachine;
        public StateMachine<Sovereign_Input> microMachine;
        public SovereignGolem sovereign;

        public SovereignPhase CurrentPhase
            => macroMachine.State is MacroState_Macro1 ? SovereignPhase.Macro1
             : macroMachine.State is MacroState_Macro2 ? SovereignPhase.Macro2
             : macroMachine.State is MacroState_Macro3 ? SovereignPhase.Macro3
             : macroMachine.State is MacroState_Macro4 ? SovereignPhase.Macro4
                                                       : SovereignPhase.None;

        public Sovereign_Input(StateMachine<Sovereign_Input> macroMachine,
                               SovereignGolem sovereign) {
            this.macroMachine = macroMachine;
            this.sovereign = sovereign;
        }

        public void InflateMicro(StateMachine<Sovereign_Input> microMachine) {
            this.microMachine = microMachine;
        }
    }
}