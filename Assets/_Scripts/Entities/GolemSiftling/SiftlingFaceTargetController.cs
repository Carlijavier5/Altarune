using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiftlingFaceTargetController : MonoBehaviour
{
    private readonly int rotateParam = Animator.StringToHash("FaceLoop"),
                         stepParam = Animator.StringToHash("RaiseLeg"),
                         jumpParam = Animator.StringToHash("Jump"),
                         directionParam = Animator.StringToHash("Direction");

    public event System.Action OnFacingEnd;

    private enum FacingState { Rotate, Step, Jump }
    private FacingState facingState;

    private enum AirborneState { Lifting, Airborne, Landing }
    private AirborneState airborneState;

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
    [Tooltip("Percent of the jumping animation at which landing occurs;\n" +
             "This adjustment may ensure landing happens at the right frame;\n" +
             "Should not be lower than the percent where the airborne trigger is;")]
    [SerializeField] private float landingPercent;
    [SerializeField] private ParticleSystemCollectionPool landingDust;
    [Tooltip("If the animation trigger for rotation does not fire, use this timer;\n" +
             "Will override the animation trigger if it happens earlier;")]
    [SerializeField] private float fallbackTimerDuration;

    private float airborneDuration;
    private Quaternion airborneStartRotation;

    private float rotationLerp;

    private float jumpBaseYPosition;
    private float jumpAltitude;
    private float landingTime;

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

            airborneState = AirborneState.Lifting;

            if (angleMag < stepDegreeRange.x) {
                facingState = FacingState.Rotate;
                animatorMain.SetTrigger(rotateParam);
                DoNextState();
            } else {
                animationRouter.OnBodyAirborne += AnimationRouter_OnBodyAirborne;

                if (angleMag <= stepDegreeRange.y) {
                    facingState = FacingState.Step;
                    animatorMain.SetTrigger(stepParam);
                } else {
                    facingState = FacingState.Jump;
                    animatorMain.SetTrigger(jumpParam);
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

    public void Stop(bool exitSequence = true) {
        animationRouter.OnBodyAirborne -= AnimationRouter_OnBodyAirborne;

        /// Reset outer siftling rotation to match body rotation;
        SetIndependentRotation(siftling.transform, Quaternion.Euler(siftlingBody.eulerAngles.x - bodyEulerOffset.x,
                                                                    siftlingBody.eulerAngles.y - bodyEulerOffset.y,
                                                                    siftlingBody.eulerAngles.z - bodyEulerOffset.z), true);

        siftling.ResetMainAnimatorSpeed();

        if (exitSequence) {
            animatorBody.enabled = true;
            enabled = false;
        }
    }

    private void DoNextState() {
        switch (airborneState) {
            case AirborneState.Lifting:
                animationRouter.OnBodyAirborne -= AnimationRouter_OnBodyAirborne;

                AnimatorStateInfo stateInfo = animatorMain.IsInTransition(0) ? animatorMain.GetNextAnimatorStateInfo(0)
                                                                             : animatorMain.GetCurrentAnimatorStateInfo(0);
                float upperLimit = facingState == FacingState.Jump ? landingPercent : 1f;
                float lowerLimit = facingState == FacingState.Rotate ? 0f : stateInfo.normalizedTime;
                float remainingTime = (upperLimit - lowerLimit) * stateInfo.length;
                animatorMain.speed = remainingTime.SafeDivide(airborneDuration);

                siftling.TogglePhaseAttackIndicators(true);
                airborneState = AirborneState.Airborne;
                break;
            case AirborneState.Airborne:
                ChooseEndingState();
                break;
            case AirborneState.Landing:
                OnFacingEnd?.Invoke();
                break;
        }
    }

    private void ChooseEndingState() {
        switch (facingState) {
            case FacingState.Rotate:
            case FacingState.Step:
                OnFacingEnd?.Invoke();
                break;
            case FacingState.Jump:
                Stop(false);
                AnimatorStateInfo stateInfo = animatorMain.IsInTransition(0) ? animatorMain.GetNextAnimatorStateInfo(0)
                                                                             : animatorMain.GetCurrentAnimatorStateInfo(0);
                landingTime = Time.time + (1 - landingPercent) * stateInfo.length * animatorMain.speed;

                airborneState = AirborneState.Landing;
                break;
        }
    }

    void Update() {
        switch (airborneState) {
            case AirborneState.Lifting:
                if (Time.time >= fallbackTime) {
                    DoNextState();
                }
                break;
            case AirborneState.Airborne:
                rotationLerp = Mathf.MoveTowards(rotationLerp, 1, Time.deltaTime.SafeDivide(airborneDuration));

                if (facingState == FacingState.Jump) {
                    siftlingBody.localPosition = new(siftlingBody.localPosition.x,
                                                     Mathf.Lerp(jumpBaseYPosition, jumpAltitude, jumpAltitudeCurve.Evaluate(rotationLerp)),
                                                     siftlingBody.localPosition.z);
                }

                Quaternion targetRotation = Quaternion.Euler((siftling.transform.eulerAngles.x + bodyEulerOffset.x),
                                                             (siftling.transform.eulerAngles.y + bodyEulerOffset.y),
                                                             (siftling.transform.eulerAngles.z + bodyEulerOffset.z));
                targetRotation = Quaternion.Dot(airborneStartRotation, targetRotation) < 0 ? new Quaternion(-targetRotation.x, -targetRotation.y,
                                                                                                            -targetRotation.z, -targetRotation.w)
                                                                                           : targetRotation;
                siftlingBody.rotation = Quaternion.Slerp(airborneStartRotation, targetRotation, rotationLerp);

                if (siftlingBody.rotation == targetRotation) {
                    if (facingState != FacingState.Rotate) {
                        landingDust.PlayAt(siftlingBody.position, Vector3.up);
                    }
                    DoNextState();
                }
                break;
            case AirborneState.Landing:
                if (Time.time >= landingTime) {
                    DoNextState();
                }
                break;
        }
    }

    private void SetIndependentRotation(Transform transform, Quaternion targetRotation, bool doReparent) {
        siftlingBody.SetParent(null);
        transform.rotation = targetRotation;
        if (doReparent) siftlingBody.SetParent(siftling.transform);
    }

    public void AnimationRouter_OnBodyAirborne() => DoNextState();

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
