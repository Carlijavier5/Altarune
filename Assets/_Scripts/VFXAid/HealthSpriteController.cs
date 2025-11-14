using System.Collections;
using UnityEngine;

public class HealthSpriteController : MonoBehaviour {

    private const string DISSOLVE_ANIMATION_PROPERTY = "PlaybackTime";

    [SerializeField] private Animator animator;
    [SerializeField] private float animationLength;
    private int dissolveParam;

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
        float target = on ? 0 : 0.98f;
        float lerpVal = animator.GetFloat(dissolveParam);
        while (Mathf.Abs(lerpVal - target) > 0) {
            lerpVal = Mathf.MoveTowards(lerpVal, target, animationLength == 0 ? Mathf.Infinity : (Time.deltaTime / animationLength));
            animator.SetFloat(dissolveParam, lerpVal);
            yield return null;
        }
    }
}