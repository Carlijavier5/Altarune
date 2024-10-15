namespace Miniboss {
    public class MinibossStateInput : StateInput {
        public Miniboss Miniboss { get; private set; }

        public MinibossStateInput(Miniboss miniboss) {
            Miniboss = miniboss;
        }
    }
}