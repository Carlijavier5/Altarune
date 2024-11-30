using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDestructor : MonoBehaviour {

    public event System.Action OnFloorCollapsed;

    [SerializeField] private Rigidbody[] sections;
    [SerializeField] private float sectionDestroyTime;
    [SerializeField] private SFXOneShot sfxFloorCollapsion;
    [SerializeField] private AnimationCurve dropIntervalCurve;

    public void CollapseFloor() {
        sfxFloorCollapsion.Play();
        StartCoroutine(ICollapseFloor());
    }

    private IEnumerator ICollapseFloor() {
        for (int i = 0; i < sections.Length; i++) {
            Rigidbody rb = sections[i];
            if (rb) {
                rb.isKinematic = false;
                Destroy(rb.gameObject, sectionDestroyTime);
                float nextDropTime = dropIntervalCurve.Evaluate(i);
                yield return new WaitForSeconds(nextDropTime);
            }
        }
        OnFloorCollapsed?.Invoke();
    }
}