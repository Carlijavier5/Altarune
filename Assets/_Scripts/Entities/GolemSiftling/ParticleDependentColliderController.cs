using System.Collections;
using UnityEngine;

public class ParticleDependentColliderController : MonoBehaviour
{
    [SerializeField] private ParticleSystem parSystem;
    [SerializeField] private Collider attackCollider;

    public void Enable() {
        StopAllCoroutines();
        attackCollider.enabled = true;
    }

    public void Disable() => StartCoroutine(IAwaitParticleStop());

    private IEnumerator IAwaitParticleStop() {
        while (parSystem.particleCount > 0) {
            yield return null;
        }
        attackCollider.enabled = false;
    }
}
