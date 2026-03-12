using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SiftlingFaceTargetController : MonoBehaviour
{
    private readonly int stepParam = Animator.StringToHash("RaiseLeg"),
                         jumpParam = Animator.StringToHash("Jump"),
                         directionParam = Animator.StringToHash("Direction");

    public event System.Action OnFacingEnd;

    private enum FacingState { Rotate, Step, Jump }
    private FacingState facingState;

    [SerializeField] private GolemSiftling siftling;
    [SerializeField] private Transform siftlingBody;
    [SerializeField] private Vector3 bodyEulerOffset;
    [SerializeField] private Animator animatorMain, animatorBody;
    [SerializeField] private SiftlingAnimationRouter animationRouter;
    [Tooltip("If target degree difference falls within this range, a step animation will be played;\n" +
             "Lower than this range and standard rotation will be played;\n" +
             "Higher than this range and a jump animation will be played;")]
    [SerializeField] private Vector2 stepDegreeRange;
    [Tooltip("How long may the siftling take to rotate towards its target;\n" +
         "For step, minimum and maximum degrees are used as defined;")]
    [SerializeField] private Vector2 stepDurationRange;
    [Tooltip("How long may the siftling take to rotate towards its target;\n" +
             "For jump, minimum used at jump minimum degree, maximum at 180°;\n" +
             "The standard rotation mode uses the lower bound of the step duration range;")]
    [SerializeField] private Vector2 airborneDurationRange;
    [Tooltip("Altitude of the jump, if a jump is required")]
    [SerializeField] private Vector2 jumpAltitudeRange;
    [SerializeField] private AnimationCurve jumpAltitudeCurve;
    [Tooltip("If the animation trigger for rotation does not fire, use this timer;\n" +
             "Will override the animation trigger if it happens earlier;")]
    [SerializeField] private float fallbackTimerDuration;

    private float airborneDuration;
    private Quaternion airborneStartRotation;

    private bool isAirborne;
    private float rotationLerp;

    private float jumpBaseYPosition;
    private float jumpAltitude;

    private float fallbackTime;

    void Awake() {
        enabled = false;
    }

    public void Play(Transform aggroTarget) {
        if (aggroTarget) {
            enabled = true;

            Vector3 direction = new(aggroTarget.position.x - siftling.transform.position.x, 0,
                                    aggroTarget.position.z - siftling.transform.position.z);
            float angle = Vector3.SignedAngle(siftling.transform.forward, direction, Vector3.up);
            float angleMag = Mathf.Abs(angle);

            animatorBody.enabled = false;
            animatorMain.SetFloat(directionParam, angle > 0 ? 1 : -1);

            if (angleMag < stepDegreeRange.x) {
                DoAirborneStates();
                facingState = FacingState.Rotate;
            } else {
                animationRouter.OnBodyAirborne += AnimationRouter_OnBodyAirborne;

                if (angleMag <= stepDegreeRange.y) {
                    animatorMain.SetTrigger(stepParam);
                    facingState = FacingState.Step;
                } else {
                    animatorMain.SetTrigger(jumpParam);
                    facingState = FacingState.Jump;
                }

                fallbackTime = Time.time + fallbackTimerDuration;
            }

            float airborneLerp = facingState switch {
                FacingState.Step => (angleMag - stepDegreeRange.x) / (stepDegreeRange.y - stepDegreeRange.x),
                FacingState.Jump => (angleMag - stepDegreeRange.y) / (180f - stepDegreeRange.y),
                _ => 0
            };

            Vector2 durationRange = facingState switch { FacingState.Jump => airborneDurationRange, _ => stepDurationRange };
            airborneDuration = Mathf.Lerp(durationRange.x, durationRange.y, airborneLerp);
            airborneStartRotation = siftlingBody.rotation;

            rotationLerp = 0;
            jumpBaseYPosition = siftlingBody.position.y;
            jumpAltitude = Mathf.Lerp(jumpAltitudeRange.x, jumpAltitudeRange.y, airborneLerp);

            SetIndependentRotation(siftling.transform, Quaternion.LookRotation(direction, Vector3.up), false);
        }
    }

    public void Stop() {
        animationRouter.OnBodyAirborne -= AnimationRouter_OnBodyAirborne;

        SetIndependentRotation(siftling.transform, Quaternion.Euler(siftlingBody.eulerAngles.x - bodyEulerOffset.x,
                                                                    siftlingBody.eulerAngles.y - bodyEulerOffset.y,
                                                                    siftlingBody.eulerAngles.z - bodyEulerOffset.z), true);

        siftling.ResetMainAnimatorSpeed();
        animatorBody.enabled = true;
        enabled = false;
    }

    private void DoAirborneStates() {
        animationRouter.OnBodyAirborne -= AnimationRouter_OnBodyAirborne;
        
        AnimatorStateInfo stateInfo = animatorMain.IsInTransition(0) ? animatorMain.GetNextAnimatorStateInfo(0)
                                                                     : animatorMain.GetCurrentAnimatorStateInfo(0);
        float remainingTime = (1f - stateInfo.normalizedTime) * stateInfo.length;
        animatorMain.speed = remainingTime.SafeDivide(airborneDuration);

        isAirborne = true;
    }

    void Update() {
        if (isAirborne) {
            rotationLerp = Mathf.MoveTowards(rotationLerp, 1, Time.deltaTime.SafeDivide(airborneDuration));

            switch (facingState) {
                case FacingState.Jump:
                    siftlingBody.localPosition = new(siftlingBody.localPosition.x,
                                                     Mathf.Lerp(jumpBaseYPosition, jumpAltitude, jumpAltitudeCurve.Evaluate(rotationLerp)),
                                                     siftlingBody.localPosition.z);
                    break;
            }

            Quaternion targetRotation = Quaternion.Euler((siftling.transform.eulerAngles.x + bodyEulerOffset.x),
                                                         (siftling.transform.eulerAngles.y + bodyEulerOffset.y),
                                                         (siftling.transform.eulerAngles.z + bodyEulerOffset.z));
            targetRotation = Quaternion.Dot(airborneStartRotation, targetRotation) < 0 ? new Quaternion(-targetRotation.x, -targetRotation.y,
                                                                                                        -targetRotation.z, -targetRotation.w)
                                                                                       : targetRotation;
            siftlingBody.rotation = Quaternion.Slerp(airborneStartRotation, targetRotation, rotationLerp);

            if (siftlingBody.rotation == targetRotation) {
                OnFacingEnd?.Invoke();
            }
        } else if (Time.time >= fallbackTime) {
            DoAirborneStates();
        }
    }

    private void SetIndependentRotation(Transform transform, Quaternion targetRotation, bool doReparent) {
        siftlingBody.SetParent(null);
        transform.rotation = targetRotation;
        if (doReparent) siftlingBody.SetParent(siftling.transform);
    }

    public void AnimationRouter_OnBodyAirborne() => DoAirborneStates();

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if (siftlingBody) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new(siftlingBody.position.x, jumpAltitudeRange.x, siftlingBody.position.z), 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new(siftlingBody.position.x, jumpAltitudeRange.y, siftlingBody.position.z), 0.1f);
            Gizmos.color = Color.white;
        }
    }
    #endif
}
