using System.Collections;
using UnityEngine;

public class SovereignEndPortal : MonoBehaviour {

    public event System.Action OnPortalShown;

    [SerializeField] private Collider eCollider;
    [SerializeField] private float growDuration;
    private float targetScale;

    void Awake() {
        targetScale = transform.localScale.x;
        transform.localScale = new Vector3(0, transform.localScale.y, 0);
    }

    public void Show() {
        gameObject.SetActive(true);
        eCollider.enabled = true;
        StartCoroutine(IShow());
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player _)) {
            GM.Instance.FinishGame();
        }
    }

    private IEnumerator IShow() {
        float lerpVal, sizeVal, timer = 0;
        while (timer < growDuration) {
            timer = Mathf.MoveTowards(timer, growDuration, Time.deltaTime);
            lerpVal = timer / growDuration;
            sizeVal = Mathf.Lerp(0, targetScale, lerpVal);
            transform.localScale = new Vector3(sizeVal, transform.localScale.y, sizeVal);
            yield return null;
        }

        OnPortalShown?.Invoke();
    }
}