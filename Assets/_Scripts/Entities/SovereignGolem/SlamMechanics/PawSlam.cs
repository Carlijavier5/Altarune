using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawSlam : MonoBehaviour {

    public event System.Action<PawSlam> OnSmashEnd;

    [SerializeField] private int damageAmount;
    [SerializeField] private float maxSize;
    [SerializeField] private SovereignSlamEpicenter outerEpicenter,
                                                    innerEpicenter;
    [SerializeField] private SFXOneShot sfxPawSlam;
    [SerializeField] private AnimationCurve growthCurve;

    private void Awake() {
        outerEpicenter.OnEntityEnter += OuterEpicenter_OnEntityEnter;
    }

    public void DoSlam(Vector3 source, float duration) {
        transform.position = source;
        gameObject.SetActive(true);
        StartCoroutine(IDoSlam(duration));
    }

    private IEnumerator IDoSlam(float duration) {
        yield return new WaitForEndOfFrame();
        ClearContacts();
        sfxPawSlam.Play();

        float lerpVal, scaleVal, timer = 0;
        while (timer < duration) {
            timer = Mathf.MoveTowards(timer, duration, Time.deltaTime);
            lerpVal = timer / duration;
            scaleVal = growthCurve.Evaluate(lerpVal) * maxSize;
            transform.localScale = Vector3.one * scaleVal;
            yield return null;
        }
        OnSmashEnd?.Invoke(this);
        gameObject.SetActive(false);
    }

    private void OuterEpicenter_OnEntityEnter(Entity entity) {
        StartCoroutine(IContactCheck(entity));
    }

    private IEnumerator IContactCheck(Entity entity) {
        yield return new WaitForFixedUpdate();
        if (!innerEpicenter.contactSet.Contains(entity)) {
            entity.TryDamage(damageAmount);
        }
    }

    private void ClearContacts() {
        outerEpicenter.Clear();
        innerEpicenter.Clear();
    }
}