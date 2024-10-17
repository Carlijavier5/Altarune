public partial class GolemSlither {
    public class State_Idle : State<GolemSlither_Input> {

        public override void Enter(GolemSlither_Input input) {
            input.golemSlither.navMeshAgent.ResetPath();

            input.golemSlither.Wait();
        }

        public override void Update(GolemSlither_Input input) { }

        public override void Exit(GolemSlither_Input input) { }
    }
}
