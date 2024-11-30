using UnityEngine;
using UnityEngine.AI;

public partial class GolemSlither {

    private const string ZIG_PARAM = "Zig";

    [Header("Zig Attack")]
    [SerializeField] private SlitherZig slitherZig;
    [SerializeField] private int zigDamage;
    [SerializeField] private float zigChargeTime, zigChargeWait;

    public class State_ZigAnticipate : State<Slither_Input> {

        public override void Enter(Slither_Input input) {
            GolemSlither gs = input.golemSlither;
            gs.navMeshAgent.ResetPath();
            gs.slitherZig.DoZig(gs.transform.position,
                                input.aggroTarget.transform.position);
            gs.animator.SetTrigger(ZIG_PARAM);
        }

        public override void Update(Slither_Input input) { }

        public override void Exit(Slither_Input input) { }
    }

    private class State_ZigCharge : State<Slither_Input> {

        private GolemSlither gs;
        private float timer;
        private int segmentIndex;

        public override void Enter(Slither_Input input) {
            gs = input.golemSlither;
            gs.navMeshAgent.enabled = false;
            segmentIndex = 0;
            DoSegment(input);
        }

        public override void Update(Slither_Input input) {
            if (segmentIndex < gs.slitherZig.segments.Length) {
                if (timer < gs.zigChargeTime) {
                    timer = Mathf.MoveTowards(timer, gs.zigChargeTime, Time.deltaTime);
                    float lerpVal = timer / gs.zigChargeTime;
                    SlitherZigSegment segment = gs.slitherZig.segments[segmentIndex];
                    gs.transform.position = Vector3.Lerp(segment.start, segment.end, lerpVal);
                } else if (timer < gs.zigChargeTime + gs.zigChargeWait) {
                    timer += Time.deltaTime;
                } else {
                    timer = 0;
                    segmentIndex++;
                    if (segmentIndex >= gs.slitherZig.segments.Length) {
                        input.stateMachine.SetState(new State_Follow());
                    } else {
                        DoSegment(input);
                    }
                }
            }
        }

        public override void Exit(Slither_Input input) {
            input.golemSlither.navMeshAgent.enabled = true;
            input.golemSlither.slitherZig.CancelZig();
        }

        private void DoSegment(Slither_Input input) {
            SlitherZigSegment segment = gs.slitherZig.segments[segmentIndex];
            Vector3 lookDir = segment.end - segment.start;
            lookDir.y = 0;
            input.golemSlither.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
            segment.DoDamage(gs.zigDamage);
        }
    }
}