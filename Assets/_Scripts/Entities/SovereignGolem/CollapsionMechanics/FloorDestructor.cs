using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDestructor : MonoBehaviour {

    public event System.Action OnFloorCollapsed;

    [SerializeField] private Rigidbody[] sections;
    [SerializeField] private float sectionDestroyTime;
    [SerializeField] private AnimationCurve dropIntervalCurve;

    void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            CollapseFloor();
        }
    }

    public void CollapseFloor() {
        StartCoroutine(ICollapseFloor());
    }

    private IEnumerator ICollapseFloor() {
        for (int i = 0; i < sections.Length; i++) {
            Rigidbody rb = sections[i];
            rb.isKinematic = false;
            Destroy(rb.gameObject, sectionDestroyTime);
            float nextDropTime = dropIntervalCurve.Evaluate(i);
            yield return new WaitForSeconds(nextDropTime);
        }
        OnFloorCollapsed?.Invoke();
    }
}