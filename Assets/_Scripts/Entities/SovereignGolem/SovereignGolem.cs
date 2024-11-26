using UnityEngine;

public enum SovereignPhase { None, Macro1, Macro2, Macro3, Macro4 };

public partial class SovereignGolem : Entity {


    private readonly StateMachine<Sovereign_Input> macroMachine = new();

    void Awake() {
        staticLaserMaster.OnAttackEnd += PhaseMaster_OnAttackEnd;
        swipingLaserMaster.OnAttackEnd += PhaseMaster_OnAttackEnd;
        palmSlamMaster.OnAttackEnd += PhaseMaster_OnAttackEnd;
        ///macroMachine.Init();
    }

    public void Animator_OnSlamLanding(LeftOrRight leftOrRight) {
        palmSlamMaster.TrySlam(leftOrRight);
    }

    public void Animator_OnCollapsionLanding() {
        collapsionSlamMaster.TryCollapsion();
    }

    private void PhaseMaster_OnAttackEnd() {
        if (macroMachine.StateInput == null) return;
        macroMachine.StateInput.microMachine
        .SetState(new State_Idle(macroMachine.StateInput.CurrentPhase));
    }
}

public partial class SovereignGolem {
    public class State_Idle : State<Sovereign_Input> {

        private readonly SovereignPhase phase;

        public State_Idle(SovereignPhase phase) {
            this.phase = phase;
        }

        public override void Enter(Sovereign_Input input) {

        }

        public override void Update(Sovereign_Input input) {
            throw new System.NotImplementedException();
        }

        public override void Exit(Sovereign_Input input) {
            throw new System.NotImplementedException();
        }
    }
}

public partial class SovereignGolem {

    [Header("Swiping Laser")]
    [SerializeField] private SovereignSwipingLaserMaster swipingLaserMaster;

    public class State_SwipingLaser : State<Sovereign_Input> {

        private readonly SovereignPhase phase;

        public State_SwipingLaser(SovereignPhase phase) {
            this.phase = phase;
        }

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.swipingLaserMaster.EnterPhase(phase);
            sg.swipingLaserMaster.DoAttack();
        }

        public override void Update(Sovereign_Input input) { }

        public override void Exit(Sovereign_Input input) { }
    }
}

public partial class SovereignGolem {

    [Header("Static Laser")]
    [SerializeField] private SovereignStaticLaserMaster staticLaserMaster;

    public class State_StaticLaser : State<Sovereign_Input> {

        private readonly SovereignPhase phase;

        public State_StaticLaser(SovereignPhase phase) {
            this.phase = phase;
        }

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.staticLaserMaster.EnterPhase(phase);
            sg.staticLaserMaster.DoAttack();
        }

        public override void Update(Sovereign_Input input) { }

        public override void Exit(Sovereign_Input input) { }
    }
}

public partial class SovereignGolem {

    [Header("Paw Slam")]
    [SerializeField] private PawSlamMaster palmSlamMaster;

    public class State_PawSlam : State<Sovereign_Input> {

        private readonly SovereignPhase phase;

        public State_PawSlam(SovereignPhase phase) {
            this.phase = phase;
        }

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.palmSlamMaster.EnterPhase(phase);
            sg.palmSlamMaster.DoAttack();
            // Broadcast SFX
        }

        public override void Update(Sovereign_Input input) { }

        public override void Exit(Sovereign_Input input) { }
    }
}

public partial class SovereignGolem {

    [Header("Collapsion Slam")]
    [SerializeField] private CollapsionSlamMaster collapsionSlamMaster;

    public class State_CollapsionSlam : State<Sovereign_Input> {

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.collapsionSlamMaster.DoAttack();
            // Broadcast SFX
        }

        public override void Update(Sovereign_Input input) { }

        public override void Exit(Sovereign_Input input) { }
    }
}