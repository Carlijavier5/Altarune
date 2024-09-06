public class Shinobi_Input : StateInput
{
    public StateMachine<Shinobi_Input> stateMachine;
    public Shinobi shinobi;
    public Entity aggroTarget;

    public Shinobi_Input(StateMachine<Shinobi_Input> stateMachine, Shinobi shinobi)
    {
        this.stateMachine = stateMachine;
        this.shinobi = shinobi;
    }

    public void SetTarget(Entity aggroTarget)
    {
        this.aggroTarget = aggroTarget;
    }
}
