using System.Collections;
using UnityEngine;

public class SiftlingDustChargeController : MonoBehaviour {

    [SerializeField] private ParticleSystem chargeDustParticles;
    [SerializeField] private float lingerDuration;
    private float dustEndTime;

    public void Play() {
        StopAllCoroutines();
        chargeDustParticles.Play();
    }

    public void Stop() {
        dustEndTime = Time.time + lingerDuration;
        StartCoroutine(IDoDustLingerTimer());
    }

    private IEnumerator IDoDustLingerTimer() {
        while (Time.time < dustEndTime) {
            yield return null;
        }
        chargeDustParticles.Stop();
    }
}