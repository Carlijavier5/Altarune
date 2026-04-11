using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiftlingWindFaceTargetController : MonoBehaviour
{
    private const float ANIMATION_COMPLETION_THRESHOLD = 0.999f;

    private readonly int stepParam = Animator.StringToHash("RaiseLeg"),
                         directionParam = Animator.StringToHash("Direction"),
                         stateLeftParam = Animator.StringToHash("WindRaiseLegLeft"),
                         stateRightParam = Animator.StringToHash("WindRaiseLegRight");
    private int ActiveLegAnimationState => turnDirectionMultiplier > 0 ? stateRightParam : stateLeftParam;

    private enum AnimationState { Transitioning, Lifting, Descending, Landing }
    private AnimationState animationState;

    private enum RotationState { Idle, Descend, Decelerate, Adjust }
    private RotationState rotationState;

    [SerializeField] private GolemSiftling siftling;
    [SerializeField] private Transform siftlingBody;
    [SerializeField] private Animator animatorMain;
    [SerializeField] private SiftlingWindFaceTargetTornadoVFXController vfxController;
    [Tooltip("At which percent of the animation should it be locked;")]
    [SerializeField] private float raisedLegPercent;
    [Tooltip("Maximum speed the siftling reaches in its descent prior to direction locking;")]
    [SerializeField] private float descentMaximumAngularSpeed;
    [Tooltip("How long until the siftling reach the maximum angular speed before the final rotation is locked;\n" +
         "Should be shorter than the prelock descent duration")]
    [SerializeField] private float descentAccelerationDurationPreLock;
    [Tooltip("The ratio between angular speed and downward speed;")]
    [SerializeField] private float descentDownwardPitch;
    [Tooltip("At which percent of the duration is the attack direction locked and indicators shown;")]
    [SerializeField] private float descentDirectionLockPercent;
    [Tooltip("Minimum remaining rotation the siftling needs to perform a smooth slowdown after reaching the floor;\n" +
             "If the remaining distance is bigger than X, we'll begin the slowdown;\n" +
             "Else, if the remaining distance is bigger than Y on the next revolution, then we'll begin the slowdown;")]
    [SerializeField] private Vector2 slowdownEnterAngleRange;
    [Tooltip("The siftling rotates slightly past the locked rotation, and then back;")]
    [SerializeField] private float descentOvershootAngle;

    private Entity aggroTarget;

    private float turnDirectionMultiplier;
    private float playbackTime, animatorSpeed;
    private float angularSpeed;
    private float upperBoundYAngle, currentYAngle, lockedTargetYAngle;
    private float slowdownBuffer, angularDeceleration;

    private float startHeight;
    private float nextHeightLimit;
    private System.Action nextHeightCallback;

    private Quaternion adjustmentStartRotation, adjustmentTargetRotation;

    void Awake() {
        enabled = false;
    }

    public void Play(Entity aggroTarget) {
        this.aggroTarget = aggroTarget;

        FlipTurnDirection();
        animatorMain.SetTrigger(stepParam);

        animationState = AnimationState.Transitioning;
        rotationState = RotationState.Idle;
        angularSpeed = 0;

        startHeight = siftlingBody.localPosition.y;
        vfxController.InitHeightScaling(startHeight);

        enabled = true;
    }

    public void Stop() {
        enabled = false;
    }

    void Update() {
        switch (animationState) {
            case AnimationState.Transitioning:
                if (!animatorMain.IsInTransition(0)) {
                    playbackTime = animatorMain.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    animatorSpeed = animatorMain.speed;

                    animatorMain.speed = 0;
                    animationState = AnimationState.Lifting;

                    SetRotationState(RotationState.Descend);
                }
                break;
            case AnimationState.Lifting:
                playbackTime = Mathf.MoveTowards(playbackTime, raisedLegPercent, Time.deltaTime * animatorSpeed);
                animatorMain.Play(ActiveLegAnimationState, 0, playbackTime);

                if (playbackTime == raisedLegPercent) {
                    animationState = AnimationState.Descending;
                }
                break;

            /// Descending is an inert state where the animation does not change;
            
            case AnimationState.Landing:
                playbackTime = Mathf.MoveTowards(playbackTime, ANIMATION_COMPLETION_THRESHOLD, Time.deltaTime * animatorSpeed);
                animatorMain.Play(ActiveLegAnimationState, 0, playbackTime);
                break;
        }
        switch (rotationState) {
            case RotationState.Descend:
                angularSpeed = Mathf.MoveTowards(angularSpeed, descentMaximumAngularSpeed, (Time.deltaTime * descentMaximumAngularSpeed).SafeDivide(descentAccelerationDurationPreLock));
                DoDownwardSpinRotation(upperBoundYAngle);
                currentYAngle %= 360;

                if ((siftlingBody.localPosition.y / startHeight) <= nextHeightLimit) {
                    nextHeightCallback?.Invoke();
                }
                break;
            case RotationState.Decelerate:
                /// We may continue rotating until we are roughly 180 degrees away from the target rotation;
                /// Reduce the velocity to zero gradually as we move toward the overshot angle with a set duration;

                float delta = Time.deltaTime * angularSpeed;
                if (slowdownBuffer > 0) {
                    slowdownBuffer = Mathf.MoveTowards(slowdownBuffer, 0, delta);
                } else {
                    angularSpeed = Mathf.MoveTowards(angularSpeed, 0, Time.deltaTime * angularDeceleration);
                }

                currentYAngle = (currentYAngle + delta * turnDirectionMultiplier) % 360;
                siftlingBody.rotation = GetBodyRotationWithEulerY(currentYAngle);

                if (angularSpeed <= 0) {
                    SetRotationState(RotationState.Adjust);
                }

                break;
            case RotationState.Adjust:
                /// Rotate with a Quaternion.Slerp into the target rotation, over the remaining duration in the animation clip;
                float lerpVal = (playbackTime - raisedLegPercent) / (ANIMATION_COMPLETION_THRESHOLD - raisedLegPercent);
                siftlingBody.rotation = Quaternion.Slerp(adjustmentStartRotation, adjustmentTargetRotation, lerpVal);
                break;
        }
    }

    private void SetRotationState(RotationState nextState) {
        switch (nextState) {
            case RotationState.Descend:
                currentYAngle = siftlingBody.eulerAngles.y;
                upperBoundYAngle = turnDirectionMultiplier > 0 ? 720 : -720;
                nextHeightLimit = descentDirectionLockPercent;

                /// This creates a micro-state within Descend, where we first hit a pre-defined
                /// height limit to lock the target rotation, and then continue rotating until we reach
                /// the bottom, at which point we transition to Decelerate;

                nextHeightCallback = () => {

                    nextHeightLimit = 0;

                    Vector3 toAggroTarget = new(siftling.transform.position.x - aggroTarget.transform.position.x, 0,
                                                siftling.transform.position.z - aggroTarget.transform.position.z);
                    lockedTargetYAngle = Quaternion.LookRotation(toAggroTarget).eulerAngles.y;

                    /// We want to activate the indicators here;

                    nextHeightCallback = () => {
                        SetRotationState(RotationState.Decelerate);
                    };
                };
                break;
            case RotationState.Decelerate:
                /// Some steps here remove lingering negatives from a leftward spin;
                
                float normalizedYAngle = ((currentYAngle % 360) + 360) % 360;

                float remainingAngle = turnDirectionMultiplier > 0 ? (lockedTargetYAngle + descentOvershootAngle - normalizedYAngle + 360) % 360
                                                                   : (normalizedYAngle - lockedTargetYAngle + descentOvershootAngle + 360) % 360;

                /// This step computes how much Y rotation is needed to be within adequate range to start a slowdown;
                /// If we are over slowdownEnterAngle.x, we enter deceleration immediately;
                /// If we are under, we will spin at full speed until we cross the remaining angle + some extra buffer angle;
                /// The goal here at that point is to get slowdownEnterAngle.y angle in between where the slowdown begins and the target;
                /// 
                bool hasEnoughRunway = (remainingAngle > slowdownEnterAngleRange.x);
                slowdownBuffer = hasEnoughRunway ? 0 : remainingAngle + 360 - slowdownEnterAngleRange.y;

                /// The deceleration formula divides by the deceleration runway, i.e. over how many angles do we decelerate to reach the target;
                /// This value is either the remaining angles if we enter immediately, or the targeted slowdownEnterAngleRange.y amount if we add another revolution;

                angularDeceleration = (angularSpeed * angularSpeed) / (2f * (hasEnoughRunway ? remainingAngle : slowdownEnterAngleRange.y));
                break;
            case RotationState.Adjust:
                adjustmentStartRotation = GetBodyRotationWithEulerY(currentYAngle);
                adjustmentTargetRotation = GetBodyRotationWithEulerY(lockedTargetYAngle);
                animationState = AnimationState.Landing;
                break;
        }
        rotationState = nextState;
    }

    private void FlipTurnDirection() {
        turnDirectionMultiplier = Random.value > 0.5f ? 1 : -1;
        animatorMain.SetFloat(directionParam, turnDirectionMultiplier);
    }

    private void DoDownwardSpinRotation(float targetAngle) {
        currentYAngle = Mathf.MoveTowards(currentYAngle, targetAngle, Time.deltaTime * angularSpeed);
        siftlingBody.rotation = GetBodyRotationWithEulerY(currentYAngle);
        float verticalSpeed = descentDownwardPitch * angularSpeed;
        siftlingBody.localPosition = Vector3.MoveTowards(siftlingBody.localPosition, new(siftlingBody.localPosition.x, 0, siftlingBody.localPosition.z), verticalSpeed * Time.deltaTime);
        vfxController.UpdateTornadoHeight(siftlingBody.localPosition.y);
    }

    private Quaternion GetBodyRotationWithEulerY(float eulerY) {
        return Quaternion.Euler(siftlingBody.eulerAngles.x, eulerY, siftlingBody.eulerAngles.z);
    } 
}
