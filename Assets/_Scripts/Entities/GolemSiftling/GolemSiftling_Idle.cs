using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSiftling {

    private const string IDLE_PARAM = "Idle";

    [Header("Idle/Roam State")]

    [SerializeField] private float maxRoamDuration;
    [SerializeField] private float roamWallBuffer;

    private class State_IdleBase : State<Siftling_Input> {
        public override void Enter(Siftling_Input input) { }

        public override void Update(Siftling_Input input) {
            if (Time.time > input.siftling.canChargeTime
                    && input.aggroTarget != null) {
                input.stateMachine.SetState(new State_Precharge(input.aggroTarget));
            }
        }

        public override void Exit(Siftling_Input input) { }
    }

    private class State_Idle : State_IdleBase {

        private float waitTimer, waitDuration;

        public override void Enter(Siftling_Input input) {
            input.siftling.MotionDriver.Set(input.siftling.navMeshAgent);
            Vector2 waitRange = input.siftling.activeConfig.waitRange;
            waitDuration = Random.Range(waitRange.x, waitRange.y);
            input.siftling.BaseAnimatorSpeed = input.siftling.activeConfig.animationSpeed;
            input.siftling.animator.SetTrigger(IDLE_PARAM);
        }

        public override void Update(Siftling_Input input) {
            base.Update(input);

            waitTimer += input.siftling.DeltaTime;
            if (waitTimer >= waitDuration) {
                input.stateMachine.SetState(new State_Roam());
            }
        }

        public override void Exit(Siftling_Input input) { }
    }

    private class State_Roam : State_IdleBase {

        private Vector3 targetLocation;
        private float endTime;

        public override void Enter(Siftling_Input input) {
            GolemSiftling golem = input.siftling;
            golem.BaseLinearSpeed = golem.activeConfig.roamSpeed;
            Vector2 distanceRange = golem.activeConfig.distanceRange;
            float distance = Random.Range(distanceRange.x, distanceRange.y);

            input.siftling.animator.SetTrigger(IDLE_PARAM);
            endTime = Time.time + golem.maxRoamDuration;

            if (PathfindingUtils.FindRandomRoamingPoint(golem.transform.position, distance,
                                                        10, out targetLocation)) {
                golem.navMeshAgent.SetDestination(targetLocation);
            } else {
                input.stateMachine.SetState(new State_Idle());
            }
        }

        public override void Update(Siftling_Input input) {
            base.Update(input);

            GolemSiftling golem = input.siftling;
            if (golem.navMeshAgent.isOnNavMesh
                    && golem.navMeshAgent.remainingDistance <= golem.navMeshAgent.stoppingDistance
                        || Time.time > endTime) {
                input.stateMachine.SetState(new State_Idle());
                input.siftling.navMeshAgent.ResetPath();
            }
        }

        public override void Exit(Siftling_Input input) {
            input.siftling.navMeshAgent.ResetPath();
        }
    }

    private const string PRE_CHARGE_PARAM = "Precharge";
    private const string CHARGE_PARAM = "Charge";

    [Header("Aggro State")]
    [SerializeField] private Vector2 chargeCDRange;

    [Header("Precharge State")]
    [SerializeField] private float prechargeTime;
    [SerializeField] private float prechargeAngularSpeed;

    [Header("Charge State")]
    [SerializeField] private float chargeTime;
    [SerializeField] private float chargeLinearSpeed;
    [SerializeField] private float chargeAngularSpeed;

    private float canChargeTime;

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
            gs.animator.SetTrigger(PRE_CHARGE_PARAM);

            prechargeEndTime = Time.time + gs.prechargeTime;

            gs.navMeshAgent.updateRotation = false;
            gs.navMeshAgent.ResetPath();
        }

        public override void Update(Siftling_Input input) {
            if (aggroTarget) {
                Vector3 lookDirection = aggroTarget.transform.position - input.siftling.transform.position;
                lookDirection.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                input.siftling.transform.rotation = Quaternion.RotateTowards(input.siftling.transform.rotation, lookRotation, Time.deltaTime * input.siftling.prechargeAngularSpeed);
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
            gs.RestartChargeCooldown();
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
            gs.animator.SetTrigger(CHARGE_PARAM);
            gs.BaseLinearSpeed = gs.chargeLinearSpeed;
            gs.BaseAngularSpeed = gs.chargeAngularSpeed;

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
        }
    }
}
