using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Meteor : MonoBehaviour {

    [SerializeField] private Transform rockTransform;
    [Header("Scale & Height")]
    [SerializeField] private AnimationCurve riseCurve;
    [SerializeField] private float floorOffset, riseHeight, vanishTime;
    [Tooltip("At which percent does the meteor achieve its maximum height;")]
    [Range(0, 1)][SerializeField] private float maxScalePercent;
    [Header("Collision Effects")]
    [SerializeField] private VisualEffect detonateVFX;
    [SerializeField] private MeteorEffectorAoE effector;
    [Header("Anticipation Area")]
    [SerializeField] private MeteorAnticipationArea anticipationArea;
    [Tooltip("What percent of the total fall time does the area account for;")]
    [Range(0, 1)][SerializeField] private float anticipationBuffer;

    public void DoRise(Vector3 position, Vector3 size, float duration) {
        gameObject.SetActive(true);
        transform.position = position;
        transform.localScale = Vector3.zero;
        StartCoroutine(IDoScale(size, duration * maxScalePercent));
        StartCoroutine(IDoRise(duration));
    }

    public void DoFall(float duration) {
        anticipationArea.DoArea(duration * anticipationBuffer);
        StartCoroutine(IDoFall(duration));
    }

    private IEnumerator IDoScale(Vector3 size, float duration) {
        Vector3 startSize = transform.localScale;
        float lerpVal, timer = 0;
        while (timer < duration) {
            timer = Mathf.MoveTowards(timer, duration, Time.deltaTime);
            lerpVal = timer / duration;
            transform.localScale = Vector3.Lerp(startSize, size, lerpVal);
            yield return null;
        }
    }

    private IEnumerator IDoRise(float duration) {
        float lerpVal, timer = 0;
        while (timer < duration) {
            timer = Mathf.MoveTowards(timer, duration, Time.deltaTime);
            lerpVal = timer / duration;
            lerpVal = riseCurve.Evaluate(lerpVal) * riseHeight;
            rockTransform.position = new Vector3(rockTransform.position.x,
                                                 Mathf.Lerp(floorOffset, riseHeight, lerpVal),
                                                 rockTransform.position.z);
            yield return null;
        }
    }

    private IEnumerator IDoFall(float duration) {
        float lerpVal, timer = 0;
        while (timer < duration) {
            timer = Mathf.MoveTowards(timer, duration, Time.deltaTime);
            lerpVal = timer / duration;
            rockTransform.position = new Vector3(rockTransform.position.x,
                                                 Mathf.Lerp(riseHeight, floorOffset, lerpVal),
                                                 rockTransform.position.z);
            yield return null;
        }
        StartCoroutine(IDoScale(Vector3.zero, vanishTime));
        if (detonateVFX) detonateVFX.Play();
        effector.DoDamage();
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + new Vector3(0, floorOffset, 0), 0.25f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + new Vector3(0, riseHeight, 0), 0.25f);
        Gizmos.color = Color.white;
    }
    #endif
}