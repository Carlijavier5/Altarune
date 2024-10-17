public partial class Golem {
    private class Golem_Input : StateInput {

        public StateMachine<Golem_Input> stateMachine;
        public Golem golem;
        public Entity aggroTarget;

        public Golem_Input(StateMachine<Golem_Input> stateMachine, Golem golem) {
            this.stateMachine = stateMachine;
            this.golem = golem;
        }

        public void SetTarget(Entity aggroTarget) {
            this.aggroTarget = aggroTarget;
        }
    }
}