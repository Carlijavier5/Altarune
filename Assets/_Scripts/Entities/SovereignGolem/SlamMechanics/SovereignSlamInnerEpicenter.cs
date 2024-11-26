using UnityEngine;

public class SovereignSlamInnerEpicenter : SovereignSlamEpicenter {

    [SerializeField] private SphereCollider outerCollider,
                                            innerCollider;
    private float thickness;

    void Awake() {
        thickness = outerCollider.radius - innerCollider.radius;
    }

    void FixedUpdate() {
        innerCollider.radius = (outerCollider.radius * outerCollider.transform.lossyScale.x
                                - thickness) / outerCollider.transform.lossyScale.x;
    }
}