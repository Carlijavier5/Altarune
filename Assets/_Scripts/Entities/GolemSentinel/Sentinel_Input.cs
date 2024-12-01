public partial class GolemSentinel {
    private class Sentinel_Input : StateInput {

        public StateMachine<Sentinel_Input> stateMachine;
        public GolemSentinel golem;
        public Entity aggroTarget;

        public Sentinel_Input(StateMachine<Sentinel_Input> stateMachine, GolemSentinel golem) {
            this.stateMachine = stateMachine;
            this.golem = golem;
        }

        public void SetTarget(Entity aggroTarget) {
            this.aggroTarget = aggroTarget;
        }
    }
}