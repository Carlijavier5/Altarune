using System.Collections.Generic;
using UnityEngine;

public enum SovereignPhase { None = 0, Macro1 = 1, Macro2 = 2, Macro3 = 3, Macro4 = 4 };

public partial class SovereignGolem : Entity {

    private const string ACTIVATION_TRIGGER = "Activate";

    [Header("General")]
    [SerializeField] private Animator animator;
    [SerializeField] private MacroStateConfiguration[] configurations;

    [Header("Spawners")]
    [SerializeField] private SovereignSpawnMaster crystalSpawner,
                                                  golemSpawner;

    private readonly Dictionary<SovereignPhase, MacroStateConfiguration> configMap = new();
    private MacroStateConfiguration activeConfig;

    private readonly StateMachine<Sovereign_Input> macroMachine = new();
    private readonly StateMachine<Sovereign_Input> microMachine = new();
    private SovereignPhase stagingPhase = SovereignPhase.None;
    private int stagingHealth;

    void Awake() {
        crystalSpawner.OnSpawnPerish += CrystalSpawner_OnSpawnPerish;
        staticLaserMaster.OnAttackEnd += PhaseMaster_OnAttackEnd;
        swipingLaserMaster.OnAttackEnd += PhaseMaster_OnAttackEnd;
        pawSlamMaster.OnAttackEnd += PhaseMaster_OnAttackEnd;
        collapsionSlamMaster.OnCollapsionEnd += PhaseMaster_OnAttackEnd;

        foreach (MacroStateConfiguration config in configurations) {
            configMap[config.phase] = config;
        }
    }

    protected override void Update() {
        base.Update();
        if (macroMachine.StateInput != null) {
            macroMachine.Update();
            if (microMachine.StateInput != null) {
                microMachine.Update();
            }
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            BeginBossFight();
        }
    }

    public void BeginBossFight() {
        Sovereign_Input input = new(macroMachine, this);
        macroMachine.Init(input, new MacroState_Inert());
        animator.SetTrigger(ACTIVATION_TRIGGER);
    }

    public void DoMacroTransition() {
        MacroState nextState = stagingPhase == SovereignPhase.Macro1 ? new MacroState_Macro1()
                             : stagingPhase == SovereignPhase.Macro2 ? new MacroState_Macro2()
                             : stagingPhase == SovereignPhase.Macro3 ? new MacroState_Macro3()
                             : stagingPhase == SovereignPhase.Macro4 ? new MacroState_Macro4()
                             : null;
        if (nextState != null) macroMachine.SetState(nextState);

        if (stagingPhase == SovereignPhase.None) {
            microMachine.Init(new Sovereign_Input(macroMachine, this),
                              new State_Roar());
            macroMachine.StateInput.InflateMicro(microMachine);
            microMachine.StateInput.InflateMicro(microMachine);
            stagingPhase = SovereignPhase.Macro1;
        }
    }

    private void CrystalSpawner_OnSpawnPerish() {
        stagingHealth--;
        Debug.Log(stagingHealth);
        if (stagingHealth <= 0) {
            stagingPhase = activeConfig.phase + 1;
        }
    }

    public void Animator_OnSlamLanding(LeftOrRight leftOrRight) {
        pawSlamMaster.TrySlam(leftOrRight);
    }

    public void Animator_OnCollapsionLanding() {
        collapsionSlamMaster.TryCollapsion();
    }

    private void PhaseMaster_OnAttackEnd() {
        if (macroMachine.StateInput == null
            || microMachine.State is State_Idle
            || microMachine.State is State_CollapsionSlam) return;
        macroMachine.StateInput.microMachine
        .SetState(new State_Idle(macroMachine.StateInput.CurrentPhase));
    }
}

public partial class SovereignGolem {
    public class State_Idle : State<Sovereign_Input> {

        private readonly SovereignPhase phase;
        private float timer;

        public State_Idle(SovereignPhase phase) {
            this.phase = phase;
        }

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            if (phase != sg.stagingPhase) {
                input.microMachine.SetState(new State_Roar());
            } else {
                Vector2 timeRange = sg.activeConfig.attackTimeRange;
                timer = Random.Range(timeRange.x, timeRange.y);
            }
        }

        public override void Update(Sovereign_Input input) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                MacroState macroState = input.macroMachine.State as MacroState;
                macroState.PickAttack(input);
            }
            if (phase != input.sovereign.stagingPhase
                && input.microMachine.State is not State_Roar) {
                input.microMachine.SetState(new State_Roar());
            }
        }

        public override void Exit(Sovereign_Input _) { }
    }
}

public partial class SovereignGolem {

    private const string ROAR_TRIGGER = "Roar";
    [SerializeField] private AnimationClip roarClip;
    [SerializeField] private SFXOneShot sfxRoar;

    public class State_Roar : State<Sovereign_Input> {

        private float timer;

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.animator.SetTrigger(ROAR_TRIGGER);
            sg.sfxRoar.Play();
        }

        public override void Update(Sovereign_Input input) {
            timer += Time.deltaTime;
            if (timer > input.sovereign.roarClip.length) {
                SovereignGolem sg = input.sovereign;
                sg.DoMacroTransition();
                input.microMachine.SetState(new State_Idle(sg.stagingPhase));
            }
        }

        public override void Exit(Sovereign_Input _) { }
    }
}

public partial class SovereignGolem {

    [Header("Swiping Laser")]
    [SerializeField] private SovereignSwipingLaserMaster swipingLaserMaster;
    [SerializeField] private SFXOneShot sfxSwipingLaserVoice;

    public class State_SwipingLaser : State<Sovereign_Input> {

        private readonly SovereignPhase phase;

        public State_SwipingLaser(SovereignPhase phase) {
            this.phase = phase;
        }

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.animator.SetTrigger(LASER_START_TRIGGER);
            sg.swipingLaserMaster.EnterPhase(phase);
            sg.swipingLaserMaster.DoAttack();
            sg.sfxSwipingLaserVoice.Play();
        }

        public override void Update(Sovereign_Input _) { }

        public override void Exit(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.animator.SetTrigger(LASER_END_TRIGGER);
        }
    }
}

public partial class SovereignGolem {

    private const string LASER_START_TRIGGER = "LaserStart";
    private const string LASER_END_TRIGGER = "LaserEnd";

    [Header("Static Laser")]
    [SerializeField] private SovereignStaticLaserMaster staticLaserMaster;
    [SerializeField] private SFXOneShot sfxStaticLaserVoice;

    public class State_StaticLaser : State<Sovereign_Input> {

        private readonly SovereignPhase phase;

        public State_StaticLaser(SovereignPhase phase) {
            this.phase = phase;
        }

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.animator.SetTrigger(LASER_START_TRIGGER);
            sg.staticLaserMaster.EnterPhase(phase);
            sg.staticLaserMaster.DoAttack();
            sg.sfxStaticLaserVoice.Play();
        }

        public override void Update(Sovereign_Input _) { }

        public override void Exit(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.animator.SetTrigger(LASER_END_TRIGGER);
        }
    }
}

public partial class SovereignGolem {

    [Header("Paw Slam")]
    [SerializeField] private PawSlamMaster pawSlamMaster;
    [SerializeField] private SFXOneShot sfxPawSlamVoice;

    public class State_PawSlam : State<Sovereign_Input> {

        private readonly SovereignPhase phase;

        public State_PawSlam(SovereignPhase phase) {
            this.phase = phase;
        }

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.pawSlamMaster.EnterPhase(phase);
            sg.pawSlamMaster.DoAttack();
            sg.sfxPawSlamVoice.Play();
        }

        public override void Update(Sovereign_Input _) { }

        public override void Exit(Sovereign_Input _) { }
    }
}

public partial class SovereignGolem {

    [Header("Collapsion Slam")]
    [SerializeField] private CollapsionSlamMaster collapsionSlamMaster;
    [SerializeField] private SFXOneShot sfxCollapsionVoice;

    public class State_CollapsionSlam : State<Sovereign_Input> {

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.collapsionSlamMaster.DoAttack();
            sg.sfxCollapsionVoice.Play();
        }

        public override void Update(Sovereign_Input _) { }

        public override void Exit(Sovereign_Input _) { }
    }
}