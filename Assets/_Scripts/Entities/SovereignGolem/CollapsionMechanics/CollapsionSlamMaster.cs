using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsionSlamMaster : MonoBehaviour {

    public event System.Action OnCollapsionEnd;

    private const string SLAM_TRIGGER = "SlamCollapsion";

    [SerializeField] private Animator animator;
    [SerializeField] private EntranceRune entranceRune;
    [SerializeField] private AreaClearer areaClearer;
    [SerializeField] private FloorDestructor floorDestructor;
    [SerializeField] private GameObject sandPit, collisionPlanes;
    [SerializeField] private SFXOneShot sfxCollapsionSlam;
    [SerializeField] private float clearDelay, dropDelay;

    void Awake() {
        floorDestructor.OnFloorCollapsed += FloorDestructor_OnFloorCollapsed;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            DoAttack();
        }
    }

    public void DoAttack() {
        StopAllCoroutines();
        animator.SetTrigger(SLAM_TRIGGER);
    }

    public void TryCollapsion() {
        sfxCollapsionSlam.Play();
        StartCoroutine(IDoCollapsion());
    }

    private IEnumerator IDoCollapsion() {
        entranceRune.CollapseRune();
        yield return new WaitForSeconds(clearDelay);
        areaClearer.ClearArea();
        sandPit.SetActive(true);
        collisionPlanes.SetActive(true);
        yield return new WaitForSeconds(dropDelay);
        floorDestructor.CollapseFloor();
    }

    private void FloorDestructor_OnFloorCollapsed() {
        OnCollapsionEnd?.Invoke();
        collisionPlanes.SetActive(false);
    }
}
