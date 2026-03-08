using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collection of particle systems to play/stop from a single endpoint;
/// </summary>
public class ParticleSystemCollection : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems;

    public void Play(bool withChildren = true) {
        foreach (ParticleSystem ps in particleSystems) {
            ps.gameObject.SetActive(true);
            ps.Play(withChildren);
        }
    }

    public void Stop(bool withChildren = true) {
        foreach (ParticleSystem ps in particleSystems) {
            ps.Stop(withChildren);
        }
    }
}
