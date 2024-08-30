using UnityEngine;

public partial class Golem {

    [Header("Aggro/Charge State")]
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
            Quaternion targetRotation = Quaternion.LookRotation(aggroTarget.transform.position - t.position, Vector3.up);
            t.rotation = Quaternion.RotateTowards(t.rotation, targetRotation, Time.deltaTime * input.golem.navMeshAgent.angularSpeed);
        }
    }
    
    private class State_AggroWait : State_Aggro {

        private float waitTimer;
        private float waitDuration;

        public override void Enter(Golem_Input input) {
            base.Enter(input);
            Golem golem = input.golem;
            waitDuration = Random.Range(golem.aggroWaitTimeRange.x,
                                        golem.aggroWaitTimeRange.y);
        }

        public override void Update(Golem_Input input) {
            base.Update(input);
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration) {
                input.stateMachine.SetState(new State_Charging());
            }
        }

        public override void Exit(Golem_Input input) { }
    }

    private class State_Charging : State_Aggro {

        private float chargeTimer;
        private Vector3 positionAnchor;

        public override void Enter(Golem_Input input) {
            base.Enter(input);
            positionAnchor = input.golem.transform.position;
        }

        public override void Update(Golem_Input input) {
            base.Update(input);
            chargeTimer = Mathf.MoveTowards(chargeTimer, input.golem.chargeTime, Time.deltaTime);
            float chargePercent = chargeTimer / input.golem.chargeTime;
            Transform t = input.golem.transform;
            t.position = new Vector3(positionAnchor.x + Random.Range(-input.golem.chargeAmplitude, 
                                                        input.golem.chargeAmplitude) * chargePercent,
                                     positionAnchor.y,
                                     positionAnchor.z + Random.Range(-input.golem.chargeAmplitude,
                                                        input.golem.chargeAmplitude) * chargePercent);
            if (chargePercent >= 1) input.stateMachine.SetState(new State_Charge());
        }

        public override void Exit(Golem_Input input) { }
    }

    private class State_Charge : State<Golem_Input> {

        private float chargeTimer;

        public override void Enter(Golem_Input input) {
            input.golem.controller.enabled = true;
            input.golem.navMeshAgent.enabled = false;
        }

        public override void Update(Golem_Input input) {
            chargeTimer += Time.deltaTime;
            Golem golem = input.golem;
            golem.controller.Move(golem.chargeSpeed * Time.deltaTime * golem.transform.forward);
            if (chargeTimer >= golem.chargeDuration) {
                input.stateMachine.SetState(input.aggroTarget == null ? new State_Idle() : new State_AggroWait());
            }
        }

        public override void Exit(Golem_Input input) {
            input.golem.controller.enabled = false;
            input.golem.navMeshAgent.enabled = true;
        }
    }
}