public partial class SovereignGolem {
    public abstract class MacroState : State<Sovereign_Input> {

        private readonly StateMachine<Sovereign_Input> stateMachine = new();
        public abstract SovereignPhase Phase { get; }

        public override void Enter(Sovereign_Input input) {
            input.InflateMacro(stateMachine);
        }

        public override void Update(Sovereign_Input input) {
            throw new System.NotImplementedException();
        }

        public override void Exit(Sovereign_Input input) {
            throw new System.NotImplementedException();
        }
    }
}
