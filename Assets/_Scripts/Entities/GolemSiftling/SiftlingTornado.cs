using System.Collections;
using UnityEngine;

public class SiftlingTornado : MonoBehaviour {

    public event System.Action OnTornadoSummoned;
    public event System.Action OnRotationSync;

    [SerializeField] private DefaultSummonProperties animationSettings;
    [SerializeField] private GolemSiftling siftling;
    [SerializeField] private Animator animatorMain;
    [SerializeField] private FollowBehavior followJoint;
    [SerializeField] private Collider[] attackColliders;
    [SerializeField] private float growTime, baseRotationSpeed;
    [SerializeField] private float preSyncAngularOffset = 15f;

    public float RotationSpeed { get; private set; }
    public float TargetRotationSpeed { get; private set; }
    public float BaseRotationSpeed { get; private set; }

    public float DirectionMult => siftling.TornadoDirectionMultiplier;
    private float cumulativeRotation;

    private float angularGrowthDuration;
    private float growthLerp;

    public float RotationSyncTarget { get; private set; }

    private float animationRevolutionRatio;
    private bool doAnimatorSync;

    private float priorSiftlingEulerY;

    void Awake() {
        TargetRotationSpeed = baseRotationSpeed;
        transform.SetParent(null);

        Vector3 tornadoOffset = transform.position - siftling.transform.position;
        tornadoOffset.y = 0;
        cumulativeRotation = Vector3.SignedAngle(siftling.transform.right, tornadoOffset.normalized, Vector3.up);

        priorSiftlingEulerY = siftling.transform.eulerAngles.y;
        siftling.OnPerish += Siftling_OnPerish;
    }

    void Update() {
        RotationSpeed = Mathf.MoveTowards(RotationSpeed, TargetRotationSpeed, Time.deltaTime.SafeDivide(angularGrowthDuration));

        float angularDelta = RotationSpeed * DirectionMult * Time.deltaTime;
        transform.Rotate(0, angularDelta, 0);

        float siftlingDelta = Mathf.DeltaAngle(priorSiftlingEulerY, siftling.transform.eulerAngles.y);
        priorSiftlingEulerY = siftling.transform.eulerAngles.y;

        angularDelta -= siftlingDelta;
        cumulativeRotation += angularDelta;

        float cumulativeRemainder = ((cumulativeRotation % 360f) + 360f) % 360f;
        float targetRemainder = ((RotationSyncTarget % 360f) + 360f) % 360f;
        float syncOffset = DirectionMult > 0 ? (targetRemainder - cumulativeRemainder + 360f) % 360f
                                             : (cumulativeRemainder - targetRemainder + 360f) % 360f;

        if (syncOffset <= preSyncAngularOffset) {
            OnRotationSync?.Invoke();
        }

        if (doAnimatorSync) {
            animatorMain.speed = RotationSpeed * animationRevolutionRatio;
        }
    }

    public void Play() {
        enabled = true;
    }

    public void Stop() {
        enabled = false;
    }

    public void ToggleAnimationSync(bool on, float clipLength = -1) {
        doAnimatorSync = on;
        if (clipLength > 0) {
            animationRevolutionRatio = clipLength / 360f;
        }
    }

    public void AdjustRotationSyncTarget(float target) {
        RotationSyncTarget = target;
    }

    public void AdjustRotationSpeed(float target, float duration) {
        /// Delta time will effectively be multiplied by the speed difference (to reach it over the given duration);
        angularGrowthDuration = 1f.SafeDivide(Mathf.Abs(RotationSpeed - target)) * duration;
        TargetRotationSpeed = target;
    }

    public void ResetRotationSpeed(float duration) {
        AdjustRotationSpeed(baseRotationSpeed, duration);
    }

    public void Toggle(bool on) {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(IToggle(on));
    }

    private IEnumerator IToggle(bool on) {
        if (on) {
            followJoint.Play();
        } else {
            foreach (Collider collider in attackColliders) {
                collider.enabled = false;
            }
        }

        float target = on ? 1 : 0;

        while (Mathf.Abs(target - growthLerp) > 0) {
            growthLerp = Mathf.MoveTowards(growthLerp, target, Time.deltaTime.SafeDivide(growTime));
            transform.localScale = new Vector3(animationSettings.growthCurveXZ.Evaluate(growthLerp),
                                               animationSettings.growthCurveY.Evaluate(growthLerp),
                                               animationSettings.growthCurveXZ.Evaluate(growthLerp));
            yield return null;
        }

        if (on) {
            OnTornadoSummoned?.Invoke();
            OnTornadoSummoned = null;

            foreach (Collider collider in attackColliders) {
                collider.enabled = true;
            }
        } else {
            followJoint.Stop();
        }
    }

    private void Siftling_OnPerish(BaseObject _) {
        StopAllCoroutines();
        StartCoroutine(IToggle(false));
    }
}
