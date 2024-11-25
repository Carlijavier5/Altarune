using UnityEngine;

public enum SovereignPhase { Macro1, Macro2, Macro3, Macro4 };

public partial class SovereignGolem : Entity {


    private readonly StateMachine<Sovereign_Input> macroMachine = new();

    void Awake() {
        
    }
}

public partial class SovereignGolem {
    public class Sovereign_Input : StateInput {

        public readonly StateMachine<Sovereign_Input> macroMachine;
        public StateMachine<Sovereign_Input> microMachine;
        public SovereignGolem sovereign;

        public Sovereign_Input(StateMachine<Sovereign_Input> macroMachine,
                               SovereignGolem sovereign) {
            this.macroMachine = macroMachine;
            this.sovereign = sovereign;
        }

        public void InflateMacro(StateMachine<Sovereign_Input> microMachine) {
            this.microMachine = microMachine;
        }
    }
}

public partial class SovereignGolem {
    public class Sovereign_Macro1 : State<Sovereign_Input> {

        private readonly StateMachine<Sovereign_Input> stateMachine = new();

        public override void Enter(Sovereign_Input input) {
            input.InflateMacro(stateMachine);
            //stateMachine.SetState();
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
    public class Sovereign_Macro2 : State<Sovereign_Input> {

        private readonly StateMachine<Sovereign_Input> stateMachine = new();

        public override void Enter(Sovereign_Input input) {
            input.InflateMacro(stateMachine);
            //stateMachine.SetState();
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
    public class Sovereign_Idle : State<Sovereign_Input> {

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

public class SovereignMacroProperties {
    [Header("General Variables")]
    public Vector2 idleWaitRange;
    [Header("Crystal Properties")]
    public int crystalAmount;
    public Vector2 crystalSpawnWaitRange;
    [Header("Spawner Properties")]
    public int maxSpawnerAmount;
    public Vector2 spawnerWaitRange;
    public bool doesContinousRespawn;
    public EntitySpawner[] spawnerRoster;
}

public partial class SovereignGolem {

    [Header("Swiping Las")]
    [SerializeField] private SovereignSwipingLaserMaster swipingLaserMaster;

    public class Sovereign_SwipingLaserAttack : State<Sovereign_Input> {

        private StateMachine<Sovereign_Input> microMachine;

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            //sg.
            //sg.swipingLaserMaster.OnAttackEnd += SwipingLaserMaster_OnAttackEnd;
            microMachine = input.microMachine;

        }

        public override void Update(Sovereign_Input input) { }

        public override void Exit(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            /*foreach (SovereignSwipingLaserController controller in sg.laserControllers) {
                controller.OnSwipeEnd -= Controller_OnSwipeEnd;
            }*/
        }

        private void SwipingLaserMaster_OnAttackEnd() {
            throw new System.NotImplementedException();
        }
    }
}