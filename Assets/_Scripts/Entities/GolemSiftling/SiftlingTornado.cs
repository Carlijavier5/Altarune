using System.Collections;
using UnityEngine;

public class SiftlingTornado : MonoBehaviour {

    public event System.Action OnTornadoSummoned;

    [SerializeField] private DefaultSummonProperties animationSettings;
    [SerializeField] private GolemSiftling siftling;
    [SerializeField] private SimpleFollow followScript;
    [SerializeField] private Collider[] attackColliders;
    [SerializeField] private float growTime, baseRotationSpeed, rotationSpeed;

    private float angularGrowthDuration, targetRotationSpeed;
    private float growthLerp;

    void Awake() {
        rotationSpeed = baseRotationSpeed;
        transform.SetParent(null);
        siftling.OnPerish += Siftling_OnPerish;
    }

    void Update() {
        rotationSpeed = Mathf.MoveTowards(rotationSpeed, targetRotationSpeed, Time.deltaTime.SafeDivide(angularGrowthDuration));
        transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
    }

    public void Play() {
        enabled = true;
    }

    public void Stop() {
        enabled = false;
    }

    public void AdjustRotationSpeed(float target, float duration) {
        /// Delta time will effectively be multiplied by the speed difference (to reach it over the given duration);
        angularGrowthDuration = 1f.SafeDivide(Mathf.Abs(rotationSpeed - target)) * duration;
        targetRotationSpeed = target;
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
            followScript.Play();
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
            followScript.Stop();
        }
    }

    private void Siftling_OnPerish(BaseObject _) {
        StopAllCoroutines();
        StartCoroutine(IToggle(false));
    }
}