public class StateMachine<T> where T : StateInput {

    public State<T> State { get; private set; }
    public T StateInput { get; private set; }

    public void Init(T input, State<T> defaultState) {
        StateInput = input;
        SetState(defaultState);
    }

    public void SetState(State<T> newState) {
        if (State != null) State.Exit(StateInput);

        State = newState;
        State.Enter(StateInput);
    }

    public void Update() {
        if (State != null) State.Update(StateInput);
    }

    public void FixedUpdate() {
        if (State != null) State.FixedUpdate(StateInput);
    }
}
