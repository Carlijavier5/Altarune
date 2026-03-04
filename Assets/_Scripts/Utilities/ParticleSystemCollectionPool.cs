using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Play one particle system from a pool,
/// as so to not override any already playing;
/// </summary>
public class ParticleSystemCollectionPool : MonoBehaviour
{
    [System.Serializable]
    public class ParticleSystemCollectionItem {
        public ParticleSystemCollection collection;
        public float duration;
    }

    [SerializeField] private ParticleSystemCollectionItem[] collectionItems;
    private readonly Dictionary<ParticleSystemCollection, float> cdValueMap = new(),
                                                                 activeCDMap = new();

    void Awake() {
        transform.SetParent(null);
        foreach (ParticleSystemCollectionItem psci in collectionItems) {
            cdValueMap.Add(psci.collection, psci.duration);
            activeCDMap.Add(psci.collection, 0);
        }
    }

    public void Play() {
        if (TryGetValidCollection(out ParticleSystemCollection freeCollection)) {
            freeCollection.Play();
            activeCDMap[freeCollection] = cdValueMap[freeCollection];
        }
    }

    public void PlayAt(Vector3 position, Vector3 normal) {
        PlayAt(position, Quaternion.LookRotation(normal, Vector3.up));
    }

    public void PlayAt(Vector3 position, Quaternion rotation) {
        if (TryGetValidCollection(out ParticleSystemCollection freeCollection)) {
            freeCollection.transform.SetPositionAndRotation(position, rotation);
            freeCollection.Play();
            activeCDMap[freeCollection] = cdValueMap[freeCollection];
        }
    }

    private bool TryGetValidCollection(out ParticleSystemCollection psc) {
        foreach (KeyValuePair<ParticleSystemCollection, float> kvp in activeCDMap) {
            if (Time.time > kvp.Value) {
                psc = kvp.Key;
                return true;
            }
        }
        psc = null;
        return false;
    }
}
