public class Shinobi_Input : StateInput
{
    public StateMachine<Shinobi_Input> stateMachine;
    public Shinobi shinobi;
    public Entity player;

    public Shinobi_Input(StateMachine<Shinobi_Input> stateMachine, Shinobi shinobi)
    {
        this.stateMachine = stateMachine;
        this.shinobi = shinobi;
    }

    public void SetPlayer(Entity player)
    {
        this.player = player;
    }
}
