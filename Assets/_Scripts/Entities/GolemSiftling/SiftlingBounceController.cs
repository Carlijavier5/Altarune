using UnityEngine;

public class SiftlingBounceController : MonoBehaviour
{
    [SerializeField] private Rigidbody parentRB, bodyRB;
    [SerializeField] private LockdownJointRigidbody lockdownJoint;
    [SerializeField] private Collider bodyCollider;
    [SerializeField] private PhysicsMaterial[] bounceMaterials;
    [SerializeField] private ContactParticlesSpawner particleSpawner;
    [SerializeField] private float changeBounceInterval;
    private int materialIndex;
    private float canChangeBounceTime;

    public void Play() {
        lockdownJoint.Play();
        bodyRB.transform.SetParent(null);

        bodyRB.isKinematic = false;
        bodyCollider.enabled = true;

        materialIndex = 0;
        bodyCollider.sharedMaterial = bounceMaterials[materialIndex];
        particleSpawner.Play();
    }

    public void Stop() {
        bodyRB.transform.SetParent(parentRB.transform);
        lockdownJoint.Stop();

        bodyRB.isKinematic = true;
        bodyCollider.enabled = false;
        particleSpawner.Stop();
    }

    void OnCollisionEnter(Collision collision) {
        if (Time.time >= canChangeBounceTime) {
            int targetIndex = Mathf.Max(materialIndex + 1, bounceMaterials.Length - 1);
            PhysicsMaterial targetMaterial = bounceMaterials[targetIndex];
            if (targetMaterial != bodyCollider.sharedMaterial) {
                bodyCollider.sharedMaterial = targetMaterial;
            }

            canChangeBounceTime = Time.time + changeBounceInterval;
        }
    }
}