using UnityEngine;

public class RBVelocityLimiter : MonoBehaviour {

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxVelocity;
    private float sqrVelocity;

    public float MaxVelocity {
        get => maxVelocity;
        set {
            maxVelocity = value;
            sqrVelocity = maxVelocity * maxVelocity;
        }
    }

    void Awake() {
        MaxVelocity = maxVelocity;
    }

    void FixedUpdate() {
        Vector3 velocity = rb.velocity;
        if (velocity.sqrMagnitude > sqrVelocity) {
            rb.velocity = velocity.normalized * maxVelocity;
        }
    }
}