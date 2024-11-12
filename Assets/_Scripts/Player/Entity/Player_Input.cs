public partial class Player {
    private class Player_Input : StateInput {

        public StateMachine<Player_Input> stateMachine;
        public Player player;

        public Player_Input(StateMachine<Player_Input> stateMachine,
                            Player player) {
            this.stateMachine = stateMachine;
            this.player = player;
        }
    }
}