using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody ragdollRigidbody;
    [SerializeField] private Collider ragdollCollider;
    [SerializeField] private float velocityMultiplier = 0.5f;

    private Vector3 previousPosition, currentPosition;
    private bool hasRootMotion;

    void Awake() {
        previousPosition = transform.position;
        currentPosition = transform.position;
    }

    void FixedUpdate() {
        previousPosition = currentPosition;
        currentPosition = transform.position;
    }

    public void Ragdoll() {
        hasRootMotion = animator.hasRootMotion;
        animator.applyRootMotion = false;

        Vector3 velocity = (currentPosition - previousPosition).SafeDivide(Time.fixedDeltaTime, 1) * velocityMultiplier;
        ragdollCollider.enabled = true;
        ragdollRigidbody.isKinematic = false;
        ragdollRigidbody.linearVelocity = velocity;
    }

    public void Restitute() {
        animator.applyRootMotion = hasRootMotion;
        ragdollCollider.enabled = false;
        ragdollRigidbody.isKinematic = true;
    }
}
