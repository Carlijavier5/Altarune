using System.Collections;
using UnityEngine;

public enum SwipeDirection { LeftRight, RightLeft }

public class SovereignSwipingLaser : SovereignLaser {

    public event System.Action OnSwipeEnd;

    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private Collider attackCollider;
    [SerializeField] private AnimationCurve rotationLerpCurve;

    public void DoTurnPath(Quaternion sourceRotation,
                           Quaternion targetRotation, float swipeTime) {
        ClearContacts();
        StopAllCoroutines();
        StartCoroutine(ITurnPath(sourceRotation, targetRotation, swipeTime));
    }

    private IEnumerator ITurnPath(Quaternion sourceRotation,
                                  Quaternion targetRotation, float swipeTime) {
        yield return new WaitForEndOfFrame();

        vfxPrefab.SetActive(true);
        attackCollider.enabled = true;
        ToggleAudio(true);

        float animationLerp, rotationLerp, timer = 0;
        while (timer < swipeTime) {
            timer = Mathf.MoveTowards(timer, swipeTime, Time.deltaTime);
            animationLerp = timer / swipeTime;
            rotationLerp = rotationLerpCurve.Evaluate(animationLerp);
            transform.rotation = Quaternion.Lerp(sourceRotation, targetRotation, rotationLerp);
            yield return null;
        }

        vfxPrefab.SetActive(false);
        attackCollider.enabled = false;
        ClearContacts();

        OnSwipeEnd?.Invoke();
        ToggleAudio(false);
    }
}