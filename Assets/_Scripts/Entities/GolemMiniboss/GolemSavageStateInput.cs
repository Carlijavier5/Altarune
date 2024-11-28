namespace GolemSavage {
    public class GolemSavageStateInput : StateInput {
        public GolemSavage GolemSavage { get; private set; }

        public GolemSavageStateInput(GolemSavage golemSavage) {
            GolemSavage = golemSavage;
        }
    }
}