using UnityEngine;

public partial class Golem {

    [Header("Aggro/Charge State")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Vector2 aggroWaitTimeRange;
    [SerializeField] private float chargeAmplitude, chargeTime,
                                   chargeSpeed, chargeDuration;

    private abstract class State_Aggro : State<Golem_Input> {

        protected Entity aggroTarget;

        public override void Enter(Golem_Input input) {
            aggroTarget = input.aggroTarget;
        }

        public override void Update(Golem_Input input) {
            Transform t = input.golem.transform;
            Vector3 lookVector = aggroTarget.transform.position - t.position;
            if (lookVector != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(aggroTarget.transform.position - t.position, Vector3.up);
                t.rotation = Quaternion.RotateTowards(t.rotation, targetRotation, input.golem.DeltaTime * input.golem.navMeshAgent.angularSpeed);
            }
        }
    }
    
    private class State_AggroWait : State_Aggro {

        private float waitTimer;
        private float waitDuration;

        public override void Enter(Golem_Input input) {
            base.Enter(input);
            Golem golem = input.golem;
            golem.navMeshAgent.ResetPath();
            waitDuration = Random.Range(golem.aggroWaitTimeRange.x,
                                        golem.aggroWaitTimeRange.y);
        }

        public override void Update(Golem_Input input) {
            base.Update(input);
            waitTimer += input.golem.DeltaTime;
            if (waitTimer >= waitDuration) {
                input.stateMachine.SetState(new State_Charging());
            }
        }

        public override void Exit(Golem_Input input) { }
    }

    private class State_Charging : State_Aggro {

        private float chargeTimer;
        private readonly float intervalVal = 0.01f;
        private int intervalCounter = 1;
        private Vector3 positionAnchor;

        public override void Enter(Golem_Input input) {
            base.Enter(input);
            positionAnchor = input.golem.bodyTransform.localPosition;
        }

        public override void Update(Golem_Input input) {
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

        public override void Exit(Golem_Input input) {
            input.golem.bodyTransform.localPosition = positionAnchor;
        }
    }

    private class State_Charge : State<Golem_Input> {

        private float chargeTimer;

        public override void Enter(Golem_Input input) {
            input.golem.controller.enabled = true;
            input.golem.navMeshAgent.enabled = false;
            input.golem.MotionDriver.Set(input.golem.controller);
        }

        public override void Update(Golem_Input input) {
            Golem golem = input.golem;
            chargeTimer += golem.DeltaTime;
            if (golem.CanMove) {
                golem.controller.Move(golem.chargeSpeed * golem.DeltaTime * golem.transform.forward);
            }

            if (chargeTimer >= golem.chargeDuration) {
                input.stateMachine.SetState(input.aggroTarget == null ? new State_Idle() : new State_AggroWait());
            }
        }

        public override void Exit(Golem_Input input) {
            input.golem.attackCollider.enabled = false;
            input.golem.attackCollider.enabled = true;

            input.golem.controller.enabled = false;
            input.golem.navMeshAgent.enabled = true;
            input.golem.MotionDriver.Set(input.golem.navMeshAgent);
        }
    }
}