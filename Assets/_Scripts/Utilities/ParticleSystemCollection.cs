using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemCollection : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems;

    public void Play() {
        foreach (ParticleSystem ps in particleSystems) {
            ps.gameObject.SetActive(true);
            ps.Play();
        }
    }

    public void Stop() {
        foreach (ParticleSystem ps in particleSystems) {
            ps.Stop();
        }
    }
}
