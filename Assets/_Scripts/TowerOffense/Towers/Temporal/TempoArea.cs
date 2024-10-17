using UnityEngine;

public class TempoArea : MonoBehaviour {

    [SerializeField] private float growSpeed;
    [SerializeField] private float timeScale;
    private Vector3 targetScale;

    void Awake() {
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    void Update() {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)) {
            baseObject.TimeScale = timeScale;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)) {
            baseObject.TimeScale = 1;
        }
    }
}