using System.Collections;
using UnityEngine;

public class EmphidianProjectile : MonoBehaviour {

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider attackCollider;
    [SerializeField] private int damageAmount;
    [SerializeField] private float linearSpeed;
    [SerializeField] private float toggleTime, lifetime;

    void Awake() {
        StartCoroutine(ILifetime());
        StartCoroutine(IToggle(true));
        Debug.Log("a bullet was born");
    }

    void FixedUpdate() {
        Vector3 targetPos = transform.position + Time.deltaTime
                          * linearSpeed * transform.forward;
        if (attackCollider.enabled) rb.MovePosition(targetPos);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)
                && baseObject.IsFaction(EntityFaction.Friendly)) {
            if (baseObject.TryDamage(damageAmount)) {
                StopAllCoroutines();
                StartCoroutine(IToggle(false));
            }
        } else if (!other.isTrigger
                   && (other.gameObject.layer & LayerUtils.GroundLayerMask) > 0) {
            Debug.Log("hello dude");
            StopAllCoroutines();
            StartCoroutine(IToggle(false));
        }
    }

    private IEnumerator ILifetime() {
        yield return new WaitForSeconds(lifetime);
        StartCoroutine(IToggle(false));
    }

    private IEnumerator IToggle(bool on) {
        attackCollider.enabled = on;
        float lerpVal, sizeMult, timer = 0,
              target = on ? toggleTime : 0;
        while (Mathf.Abs(target - timer) > 0) {
            timer = Mathf.MoveTowards(timer, target, Time.deltaTime);
            lerpVal = timer / toggleTime;
            sizeMult = Mathf.Lerp(0, 1, lerpVal);
            transform.localScale = Vector3.one * sizeMult;
            yield return null;
        }
    }
}