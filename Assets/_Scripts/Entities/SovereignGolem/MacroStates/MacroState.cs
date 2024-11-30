public partial class SovereignGolem {

    private enum SovereignAttack { StaticLaser, SwipingLaser, PawSlam }

    public abstract class MacroState : State<Sovereign_Input> {

        public abstract SovereignPhase Phase { get; }

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.crystalSpawner.EnterPhase(Phase);
            sg.golemSpawner.EnterPhase(Phase);
            if (sg.configMap.ContainsKey(Phase)) {
                sg.activeConfig = sg.configMap[Phase];
                sg.stagingHealth = sg.activeConfig.crystalHealth;
            }
        }

        public override void Update(Sovereign_Input input) { }

        public override void Exit(Sovereign_Input input) { }

        public virtual void PickAttack(Sovereign_Input input) { }
    }
}
