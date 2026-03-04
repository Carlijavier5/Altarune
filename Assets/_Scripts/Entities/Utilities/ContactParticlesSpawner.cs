using System.Collections.Generic;
using UnityEngine;

public class ContactParticlesSpawner : MonoBehaviour {

    public event System.Action OnContact;

    [SerializeField] private ParticleSystemCollectionPool particlePool;
    [SerializeField] private LayerMask contactLayers;
    [SerializeField] private float particleSpawnCD;

    private float canSpawnTime;

    public void Play() {
        enabled = true;
    }

    public void Stop() {
        enabled = false;
    }

    void OnCollisionEnter(Collision collision) {
        bool canSpawnParticles = Time.time > canSpawnTime
                                    && contactLayers.Contains(collision.gameObject.layer);
        if (canSpawnParticles) {
            List<ContactPoint> contacts = new();
            int contactCount = collision.GetContacts(contacts);

            canSpawnTime = Time.time + particleSpawnCD;

            Vector3 averagePoint = Vector3.zero;
            for (int i = 0; i < contactCount; i++) {
                Vector3 position = contacts[i].point + contacts[i].normal * 0.05f;
                Vector3 normal = transform.position - contacts[i].point;
                particlePool.PlayAt(position, normal);

                OnContact?.Invoke();
            }
        }
    }
}