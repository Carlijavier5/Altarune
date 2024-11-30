public class Slither_Input : StateInput
{
    public StateMachine<Slither_Input> stateMachine;
    public GolemSlither golemSlither;
    public Entity aggroTarget;

    public Slither_Input(StateMachine<Slither_Input> stateMachine, GolemSlither golemSlither) {
        this.stateMachine = stateMachine;
        this.golemSlither = golemSlither;
    }

    public void SetAggroTarget(Entity aggroTarget) {
        this.aggroTarget = aggroTarget;
    }
}