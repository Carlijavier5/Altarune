using System.Collections;
using UnityEngine;

public class HealthSpriteController : MonoBehaviour {

    private const string DISSOLVE_ANIMATION_PROPERTY = "PlaybackTime";

    [SerializeField] private Animator animator;
    [SerializeField] private float animationLength;
    private int dissolveParam;
    private float timer;

    void Awake() {
        dissolveParam = Animator.StringToHash(DISSOLVE_ANIMATION_PROPERTY);
    }

    public void Break() {
        StopAllCoroutines();
        StartCoroutine(IMaterialize(false));
    }

    public void Restore() {
        StopAllCoroutines();
        StartCoroutine(IMaterialize(true));
    }

    private IEnumerator IMaterialize(bool on) {
        float lerpVal, target = on ? 0 : animationLength;
        while (Mathf.Abs(timer - target) > Mathf.Epsilon) {
            timer = Mathf.MoveTowards(timer, target, Time.deltaTime);
            lerpVal = Mathf.Clamp(timer / animationLength, 0, 0.98f);
            animator.SetFloat(dissolveParam, lerpVal);
            yield return null;
        }
    }
}