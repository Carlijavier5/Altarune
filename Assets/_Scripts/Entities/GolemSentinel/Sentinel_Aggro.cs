using UnityEngine;

public partial class GolemSentinel {

    [Header("Aggro/Charge State")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Vector2 aggroWaitTimeRange;
    [SerializeField] private float chargeAmplitude, chargeTime,
                                   chargeSpeed, chargeDuration;

    private abstract class State_Aggro : State<Sentinel_Input> {

        protected Entity aggroTarget;

        public override void Enter(Sentinel_Input input) {
            aggroTarget = input.aggroTarget;
        }

        public override void Update(Sentinel_Input input) {
            Transform t = input.golem.transform;
            if (aggroTarget != null) {
                Vector3 lookVector = aggroTarget.transform.position - t.position;
                if (lookVector != Vector3.zero) {
                    Quaternion targetRotation = Quaternion.LookRotation(aggroTarget.transform.position - t.position, Vector3.up);
                    t.rotation = Quaternion.RotateTowards(t.rotation, targetRotation, input.golem.DeltaTime * input.golem.navMeshAgent.angularSpeed);
                }
            } else input.golem.UpdateAggro();
        }
    }
    
    private class State_AggroWait : State_Aggro {

        private float waitTimer;
        private float waitDuration;

        public override void Enter(Sentinel_Input input) {
            base.Enter(input);
            GolemSentinel golem = input.golem;
            golem.navMeshAgent.ResetPath();
            waitDuration = Random.Range(golem.aggroWaitTimeRange.x,
                                        golem.aggroWaitTimeRange.y);
        }

        public override void Update(Sentinel_Input input) {
            base.Update(input);
            waitTimer += input.golem.DeltaTime;
            if (waitTimer >= waitDuration) {
                input.stateMachine.SetState(new State_Charging());
            }
        }

        public override void Exit(Sentinel_Input input) { }
    }

    private class State_Charging : State_Aggro {

        private float chargeTimer;
        private readonly float intervalVal = 0.01f;
        private int intervalCounter = 1;
        private Vector3 positionAnchor;

        public override void Enter(Sentinel_Input input) {
            base.Enter(input);
            positionAnchor = input.golem.bodyTransform.localPosition;
        }

        public override void Update(Sentinel_Input input) {
            base.Update(input);
            chargeTimer = Mathf.MoveTowards(chargeTimer, input.golem.chargeTime, input.golem.DeltaTime);
            float chargePercent = chargeTimer / input.golem.chargeTime;
            Transform t = input.golem.bodyTransform;
            if (chargePercent > intervalVal * intervalCounter) {
                t.localPosition = new Vector3(positionAnchor.x + Random.Range(-input.golem.chargeAmplitude,
                                                                              input.golem.chargeAmplitude) * chargePercent,
                                              positionAnchor.y,
                                              positionAnchor.z + Random.Range(-input.golem.chargeAmplitude,
                                                                              input.golem.chargeAmplitude) * chargePercent);
                intervalCounter++;
            }
            if (chargePercent >= 1) input.stateMachine.SetState(new State_Charge());
        }

        public override void Exit(Sentinel_Input input) {
            input.golem.bodyTransform.localPosition = positionAnchor;
        }
    }

    private class State_Charge : State<Sentinel_Input> {

        private float chargeTimer;

        public override void Enter(Sentinel_Input input) {
            input.golem.controller.enabled = true;
            input.golem.navMeshAgent.enabled = false;
            input.golem.MotionDriver.Set(input.golem.controller);
        }

        public override void Update(Sentinel_Input input) {
            GolemSentinel golem = input.golem;
            chargeTimer += golem.DeltaTime;
            if (golem.CanMove) {
                golem.controller.Move(golem.chargeSpeed * golem.DeltaTime * golem.transform.forward);
            }

            if (chargeTimer >= golem.chargeDuration) {
                golem.UpdateAggro();
                //input.stateMachine.SetState(input.aggroTarget == null ? new State_Idle() : new State_AggroWait());
            }
        }

        public override void Exit(Sentinel_Input input) {
            input.golem.attackCollider.enabled = false;
            input.golem.attackCollider.enabled = true;

            input.golem.controller.enabled = false;
            input.golem.navMeshAgent.enabled = true;
            input.golem.MotionDriver.Set(input.golem.navMeshAgent);
        }
    }

    private class State_Slash : State_Aggro {

        [Header("Slash")]
        [SerializeField] private MeleeAreaWarning meleeWarningArea;

        public override void Exit(Sentinel_Input input) {
            input.golem.UpdateAggro();
        }
    }
}
