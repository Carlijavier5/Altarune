public class StateMachine<T> where T : StateInput {

    public State<T> State { get; private set; }
    public T StateInput { get; private set; }

    public void Init(T input, State<T> defaultState) {
        StateInput = input;
        SetState(defaultState);
    }

    public void SetState(State<T> newState) {
        State?.Exit(StateInput);

        State = newState;
        State.Enter(StateInput);
    }

    public void Update() {
        State?.Update(StateInput);
    }

    public void FixedUpdate() {
        State?.FixedUpdate(StateInput);
    }
}
