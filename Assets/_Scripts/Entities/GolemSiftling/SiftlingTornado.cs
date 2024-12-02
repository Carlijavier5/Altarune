using System.Collections;
using UnityEngine;

public class SiftlingTornado : MonoBehaviour {

    public event System.Action OnTornadoSummoned;

    [SerializeField] private DefaultSummonProperties animationSettings;
    [SerializeField] private GolemSiftling siftling;
    [SerializeField] private Collider[] attackColliders;
    [SerializeField] private float growTime, rotationSpeed;

    void Awake() {
        siftling.OnPerish += Siftling_OnPerish;
    }

    void Update() {
        transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
    }

    private void Siftling_OnPerish(BaseObject _) {
        StopAllCoroutines();
        StartCoroutine(IToggle(false));
    }

    public void Toggle(bool on) {
        gameObject.SetActive(true);
        StartCoroutine(IToggle(on));
    }

    private IEnumerator IToggle(bool on) {
        float target = on ? growTime : 0;

        float lerpVal, growTimer = growTime - target;
        while (Mathf.Abs(growTimer - target) > Mathf.Epsilon) {
            growTimer = Mathf.MoveTowards(growTimer, target, Time.deltaTime);
            lerpVal = growTimer / growTime;
            transform.localScale = new Vector3(animationSettings.growthCurveXZ.Evaluate(lerpVal),
                                               animationSettings.growthCurveY.Evaluate(lerpVal),
                                               animationSettings.growthCurveXZ.Evaluate(lerpVal));
            yield return null;
        }

        if (on) {
            OnTornadoSummoned?.Invoke();
            foreach (Collider collider in attackColliders) {
                collider.enabled = true;
            }
        } else Destroy(gameObject, 2);
    }
}