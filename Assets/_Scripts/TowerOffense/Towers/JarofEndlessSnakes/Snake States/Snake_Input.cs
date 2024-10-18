public class Snake_Input : StateInput {

    public StateMachine<Snake_Input> stateMachine;
    public Snake snake;
    public Entity aggroTarget;

    public Snake_Input(StateMachine<Snake_Input> stateMachine, Snake snake) {
        this.stateMachine = stateMachine;
        this.snake = snake;
    }

    public void SetTarget(Entity aggroTarget) {
        this.aggroTarget = aggroTarget;
    }
}