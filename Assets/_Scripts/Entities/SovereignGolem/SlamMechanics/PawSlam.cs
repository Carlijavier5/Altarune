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
    
    [SerializeField] private ParticleSystem smoke;

    private void Awake() {
        transform.SetParent(null);
        outerEpicenter.OnObjectEnter += OuterEpicenter_OnObjectEnter;
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
        smoke.transform.position = transform.position + new Vector3(0f, 1f, 0f);
        smoke.Play();
        GM.CameraShakeManager.DoCameraShake(10f, 1f);

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

    private void OuterEpicenter_OnObjectEnter(BaseObject baseObject) {
        StartCoroutine(IContactCheck(baseObject));
    }

    private IEnumerator IContactCheck(BaseObject baseObject) {
        yield return new WaitForFixedUpdate();
        if (!innerEpicenter.contactSet.Contains(baseObject)) {
            baseObject.TryDamage(damageAmount);
        }
    }

    private void ClearContacts() {
        outerEpicenter.Clear();
        innerEpicenter.Clear();
    }
}