using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SniperProjectileController : MonoBehaviour {
    [SerializeField] ParticleSystem particle;
    ParticleSystem Particle {
        get => particle;
        set => particle = value;
    }

    [SerializeField] ParticleSystem smokeRelease;

    [SerializeField] Transform launchPoint;

    public void Fire() {
        if (Particle) {
            var shape = Particle.shape;
            shape.position = particle.transform.InverseTransformPoint(launchPoint.position);
            
            particle.Play();
            smokeRelease.Play();
        }
    }
    
}
