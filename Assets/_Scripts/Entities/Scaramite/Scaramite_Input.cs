public class Scaramite_Input : StateInput {

    public StateMachine<Scaramite_Input> stateMachine;
    public Scaramite scaramite;
    public Entity aggroTarget;

    public Scaramite_Input(StateMachine<Scaramite_Input> stateMachine, Scaramite scaramite) {
        this.stateMachine = stateMachine;
        this.scaramite = scaramite;
    }

    public void SetTarget(Entity aggroTarget) {
        this.aggroTarget = aggroTarget;
    }
}