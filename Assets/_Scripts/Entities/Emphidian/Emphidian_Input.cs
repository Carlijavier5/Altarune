public partial class Emphidian {
    public class Emphidian_Input : StateInput {

        public StateMachine<Emphidian_Input> stateMachine;
        public Emphidian emphidian;
        public Entity aggroTarget;

        public Emphidian_Input(StateMachine<Emphidian_Input> stateMachine, Emphidian emphidian) {
            this.stateMachine = stateMachine;
            this.emphidian = emphidian;
        }

        public void SetTarget(Entity aggroTarget) {
            this.aggroTarget = aggroTarget;
        }
    }
}