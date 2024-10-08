public class GolemSlither_Input : StateInput
{
    public StateMachine<GolemSlither_Input> stateMachine;
    public GolemSlither golemSlither;
    public Entity player;

    public GolemSlither_Input(StateMachine<GolemSlither_Input> stateMachine, GolemSlither golemSlither) {
        this.stateMachine = stateMachine;
        this.golemSlither = golemSlither;
    }

    public void SetPlayer(Entity player) {
        this.player = player;
    }
}
