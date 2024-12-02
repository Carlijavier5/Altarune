using System.Collections;
using UnityEngine;

public partial class GolemSavage {

    private const string SPIN_START_PARAM = "SpinStart",
                         SPIN_END_PARAM = "SpinEnd";

    [Header("Spin")]
    [SerializeField] private SavageSpinEffector spinEffector;
    [SerializeField] private AnimationClip spinStartClip;
    [SerializeField] private float spinRaycastDistance;

    private class State_Spin : State<Savage_Input> {

        private enum SpinState { Start, Loop, Stop, Done }

        private GolemSavage gs;
        private SpinState spinState;

        private Vector3 RandomDirection {
            get {
                float randomAngle = Random.Range(0f, 360f);
                return Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
            }
        }

        private float spinSpeed;
        private Vector3 moveDirection;
        private float timer;

        public override void Enter(Savage_Input input) {
            gs = input.savage;
            gs.navMeshAgent.SetDestination(gs.transform.position);
            gs.navMeshAgent.velocity = Vector3.zero;

            moveDirection = RandomDirection;
            gs.TryToggleIFrame(true);
            gs.animator.SetTrigger(SPIN_START_PARAM);
        }

        public override void Update(Savage_Input input) {
            timer += Time.deltaTime;

            switch (spinState) {
                case SpinState.Start:
                    if (timer > gs.spinStartClip.length) {
                        spinState = SpinState.Loop;
                        gs.spinEffector.ToggleDamage(true);
                        timer = 0;
                    } break;
                case SpinState.Loop:
                    if (timer < gs.activeConfig.spinDuration) {
                        float lerpVal = Mathf.Clamp01(timer / gs.activeConfig.maxSpinSpeedDelay);
                        spinSpeed = Mathf.Lerp(0, gs.activeConfig.maxSpinSpeed, lerpVal);
                    } else {
                        spinState = SpinState.Stop;
                        timer = 0;
                    } break;
                case SpinState.Stop:
                    if (timer < gs.activeConfig.stopDuration) {
                        float lerpVal = timer / gs.activeConfig.stopDuration;
                        spinSpeed = Mathf.Lerp(gs.activeConfig.maxSpinSpeed, 0, lerpVal);
                    } else {
                        gs.spinEffector.ToggleDamage(false);
                        gs.animator.SetTrigger(SPIN_END_PARAM);
                        spinState = SpinState.Done;
                    } break;
            }

            if (Physics.Raycast(gs.transform.position, moveDirection,
                                out RaycastHit hit, gs.spinRaycastDistance)) {
                moveDirection = Vector3.Reflect(moveDirection, hit.normal);
                float randomVariation = Random.Range(-45f, 45f);
                moveDirection = Quaternion.Euler(0, randomVariation, 0) * moveDirection;
                GM.CameraShakeManager.DoCameraShake();
            }

            gs.navMeshAgent.Move(Time.deltaTime * spinSpeed * moveDirection);
        }

        public override void Exit(Savage_Input input) {
            gs.TryToggleIFrame(false);
        }
    }
}