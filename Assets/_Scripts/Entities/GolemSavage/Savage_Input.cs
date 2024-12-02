public partial class GolemSavage {
    public class Savage_Input : StateInput {

        public StateMachine<Savage_Input> macroMachine,
                                            microMachine;
        public GolemSavage savage;
        public Entity aggroTarget;

        public Savage_Input(StateMachine<Savage_Input> macroMachine,
                            StateMachine<Savage_Input> microMachine,
                            GolemSavage savage) {
            this.macroMachine = macroMachine;
            this.microMachine = microMachine;
            this.savage = savage;
        }

        public void SetAggroTarget(Entity aggroTarget) {
            this.aggroTarget = aggroTarget;
        }
    }
}