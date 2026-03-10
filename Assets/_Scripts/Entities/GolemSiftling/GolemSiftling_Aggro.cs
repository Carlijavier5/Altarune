using UnityEngine;

public partial class GolemSiftling {

    private const string PRE_CHARGE_PARAM = "Precharge";
    private const string CHARGE_PARAM = "Charge";

    [Header("Aggro State")]
    [SerializeField] private GolemSiftlingChargeVFXController vfxController;

    [Header("Precharge State")]
    [SerializeField] private float prechargeTime;
    [SerializeField] private float prechargeAngularSpeed;

    [Header("Charge State")]
    [SerializeField] private ParticleDependentColliderController attackColliderController;
    [SerializeField] private float chargeTime;
    [SerializeField] private float chargeLinearSpeed;
    [SerializeField] private float chargeAngularSpeed;

    private float canAttackTime;

    private class State_Aggro : State<Siftling_Input> {

        protected Siftling_Input input;
        protected Entity aggroTarget;

        public State_Aggro(Entity aggroTarget) {
            this.aggroTarget = aggroTarget;
        }

        public override void Enter(Siftling_Input input) {
            this.input = input;
            input.siftling.OnLongPush += Siftling_OnLongPush;
        }

        public override void Update(Siftling_Input input) { }

        public override void Exit(Siftling_Input input) {
            input.siftling.OnLongPush -= Siftling_OnLongPush;
        }

        public virtual void PropagateAggroExit(Entity entity) { }

        private void Siftling_OnLongPush() {
            input.siftling.IsStunned = true;
        }
    }

    private class State_Precharge : State_Aggro {

        private float prechargeEndTime;

        public State_Precharge(Entity aggroTarget) : base(aggroTarget) { }

        public override void Enter(Siftling_Input input) {
            base.Enter(input);

            GolemSiftling gs = input.siftling;
            gs.animatorMain.SetTrigger(PRE_CHARGE_PARAM);

            gs.vfxController.DoPrecharge(gs.activeConfig.attackAnticipationDuration);
            prechargeEndTime = Time.time + gs.activeConfig.attackAnticipationDuration;

            gs.navMeshAgent.updateRotation = false;
            gs.navMeshAgent.ResetPath();
        }

        public override void Update(Siftling_Input input) {
            if (aggroTarget) {
                Vector3 lookDirection = aggroTarget.transform.position - input.siftling.transform.position;
                lookDirection.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                input.siftling.transform.rotation = Quaternion.RotateTowards(input.siftling.transform.rotation, lookRotation, Time.deltaTime * input.siftling.activeConfig.lookAngularSpeed);
            }

            if (Time.time > prechargeEndTime) {
                input.stateMachine.SetState(new State_Charge(aggroTarget));
            }
        }

        public override void Exit(Siftling_Input input) {
            base.Exit(input);

            GolemSiftling gs = input.siftling;
            gs.navMeshAgent.ResetPath();
            gs.navMeshAgent.updateRotation = true;

            gs.vfxController.CancelPrecharge();
        }

        public override void PropagateAggroExit(Entity entity) {
            if (entity == aggroTarget) {
                input.stateMachine.SetState(new State_Idle());
            }
        }
    }

    private class State_Charge : State_Aggro {

        private float chargeEndTime;

        public State_Charge(Entity aggroTarget) : base(aggroTarget) { }

        public override void Enter(Siftling_Input input) {
            base.Enter(input);

            GolemSiftling gs = input.siftling;
            gs.animatorMain.SetTrigger(CHARGE_PARAM);
            gs.BaseLinearSpeed = gs.chargeLinearSpeed;
            gs.BaseAngularSpeed = gs.chargeAngularSpeed;

            gs.vfxController.DoCharge();
            gs.attackColliderController.Enable();
            chargeEndTime = Time.time + gs.chargeTime;

            gs.navMeshAgent.autoBraking = false;
            gs.navMeshAgent.stoppingDistance = 0;
        }

        public override void Update(Siftling_Input input) {
            if (aggroTarget) {
                input.siftling.navMeshAgent.SetDestination(aggroTarget.transform.position);
            }

            if (Time.time > chargeEndTime) {
                input.stateMachine.SetState(new State_Idle());
            }
        }

        public override void Exit(Siftling_Input input) {
            base.Exit(input);

            input.siftling.BaseAngularSpeed = input.siftling.idleAngularSpeed;
            input.siftling.navMeshAgent.autoBraking = true;
            input.siftling.navMeshAgent.stoppingDistance = input.siftling.idleStoppingDistance;
            input.siftling.navMeshAgent.ResetPath();

            input.siftling.RestartAttackCooldown();
            input.siftling.vfxController.CancelAll();
            input.siftling.attackColliderController.Disable();
        }
    }
}
