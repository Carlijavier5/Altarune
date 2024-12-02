using UnityEngine;

public partial class GolemSavage {

    private const string IDLE_PARAM = "Idle";

    [Header("Idle/Follow State")]

    [SerializeField] private Vector2 roamWaitTimeRange;
    [SerializeField] private float maxRoamDuration;

    private class State_Idle : State<Savage_Input> {

        private readonly SavagePhase phase;
        private readonly float attackTime;
        private float attackTimer; 

        public State_Idle(SavagePhase phase, float attackTime,
                          float attackTimer = 0) {
            this.phase = phase;
            this.attackTimer = attackTimer;
            this.attackTime = attackTime;
        }

        private float roamSwitchTimer, waitDuration;

        public override void Enter(Savage_Input input) {
            GolemSavage gs = input.savage;
            Vector2 waitRange = input.savage.roamWaitTimeRange;
            waitDuration = Random.Range(waitRange.x, waitRange.y);
            gs.animator.SetTrigger(IDLE_PARAM);

            if (phase != gs.stagingPhase) {
                input.microMachine.SetState(new State_Ascension());
            }
        }

        public override void Update(Savage_Input input) {
            roamSwitchTimer += Time.deltaTime;
            if (roamSwitchTimer >= waitDuration) {
                input.microMachine.SetState(new State_Roam(phase, attackTime,
                                                           attackTimer));
            }

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackTime) {
                PhaseState macroState = input.macroMachine.State as PhaseState;
                macroState.PickAttack(input);
            }

            if (phase != input.savage.stagingPhase
                    && input.microMachine.State is not State_Ascension) {
                input.microMachine.SetState(new State_Ascension());
            }

            if (input.aggroTarget) {
                Vector3 dir = input.aggroTarget.transform.position
                            - input.savage.transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(dir, Vector3.up);
                input.savage.transform.rotation = Quaternion.RotateTowards(
                    input.savage.transform.rotation, lookRotation,
                    input.savage.navMeshAgent.angularSpeed * Time.deltaTime
                );
            }
        }

        public override void Exit(Savage_Input _) { }
    }

    private class State_Roam : State<Savage_Input> {

        private readonly SavagePhase phase;
        private readonly float attackTime;
        private float attackTimer;

        public State_Roam(SavagePhase phase, float attackTime,
                          float attackTimer = 0) {
            this.phase = phase;
            this.attackTimer = attackTimer;
            this.attackTime = attackTime;
        }

        private float timer;

        public override void Enter(Savage_Input input) {
            GolemSavage gs = input.savage;
            gs.BaseLinearSpeed = gs.activeConfig.roamSpeed;

            if (phase != gs.stagingPhase) {
                input.microMachine.SetState(new State_Ascension());
            }
        }

        public override void Update(Savage_Input input) {
            GolemSavage gs = input.savage;
            timer += Time.deltaTime;

            if (input.aggroTarget) {
                gs.navMeshAgent.SetDestination(input.aggroTarget.transform.position);
            } else {
                input.microMachine.SetState(new State_Idle(phase, attackTime, attackTimer));
            }

            if (timer >= gs.maxRoamDuration) {
                input.microMachine.SetState(new State_Idle(phase, attackTime, attackTimer));
                gs.navMeshAgent.ResetPath();
            }

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackTime) {
                PhaseState macroState = input.macroMachine.State as PhaseState;
                macroState.PickAttack(input);
            }

            if (phase != input.savage.stagingPhase
                    && input.microMachine.State is not State_Ascension) {
                input.microMachine.SetState(new State_Ascension());
            }
        }

        public override void Exit(Savage_Input _) { }
    }
}