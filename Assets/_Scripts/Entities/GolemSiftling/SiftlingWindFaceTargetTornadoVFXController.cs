using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiftlingWindFaceTargetTornadoVFXController : MonoBehaviour
{
    [System.Serializable]
    private class ParticleSystemStopGroup {
        public ParticleSystem[] particles;
        public float stopThreshold;
    }

    [SerializeField] private Collider attackCollider;
    [SerializeField] private float contactAttackStopThreshold;
    [SerializeField] private Transform tornadoRoot;
    [SerializeField] private Vector3 groundTornadoScale;
    [SerializeField] private ParticleSystemStopGroup[] particleStopGroups;
    [SerializeField] private ParticleSystem[] particlesToColor;
    [SerializeField] private Color particleColorTarget;
    [SerializeField] private ParticleSystem[] particlesToRotate;
    [SerializeField] private float tornadoMaxAngularSpeed;
    [SerializeField] private float groundOffset;

    private float[] initialYRotationModules;
    private Color[] initialColorModules;

    private Vector3 baseTornadoScale;
    private float siftlingStartHeight;
    private float tornadoStartHeight;

    void Awake() {
        baseTornadoScale = tornadoRoot.localScale;
        tornadoStartHeight = tornadoRoot.position.y;

        initialYRotationModules = new float[particlesToRotate.Length];
        for (int i = 0; i < particlesToRotate.Length; i++) {
            initialYRotationModules[i] = particlesToRotate[i].rotationOverLifetime.yMultiplier;
        }

        initialColorModules = new Color[particlesToColor.Length];
        for (int i = 0; i < particlesToColor.Length; i++) {
            initialColorModules[i] = particlesToColor[i].main.startColor.color;
        }
    }

    public void InitHeightScaling(float siftlingBodyY) {
        siftlingStartHeight = siftlingBodyY;
    }

    public void UpdateTornadoHeight(float siftlingBodyY) {
        float lerpVal = (siftlingStartHeight - siftlingBodyY) / siftlingStartHeight;
        tornadoRoot.localScale = Vector3.Lerp(baseTornadoScale, groundTornadoScale, lerpVal);
        tornadoRoot.position = new(tornadoRoot.position.x, Mathf.Lerp(tornadoStartHeight, tornadoStartHeight + groundOffset, lerpVal), tornadoRoot.position.z);

        if (lerpVal >= contactAttackStopThreshold) {
            attackCollider.enabled = false;
        }

        for (int i = 0; i < particlesToColor.Length; i++) {
            ParticleSystem.MainModule main = particlesToColor[i].main;
            ParticleSystem.MinMaxGradient gradient = main.startColor;
            gradient.color = Color.Lerp(initialColorModules[i], particleColorTarget, lerpVal);
            main.startColor = gradient;
        }

        for (int i = 0; i < particlesToRotate.Length; i++) {
            ParticleSystem.RotationOverLifetimeModule rotation = particlesToRotate[i].rotationOverLifetime;
            rotation.y = Mathf.Lerp(initialYRotationModules[i], tornadoMaxAngularSpeed * Mathf.Deg2Rad, lerpVal);
        }

        foreach (ParticleSystemStopGroup group in particleStopGroups) {
            if (lerpVal > group.stopThreshold) {
                foreach (ParticleSystem ps in group.particles) {
                    ParticleSystem.EmissionModule emission = ps.emission;
                    emission.enabled = false;
                }
            }
        }
    }

    public void ResetVFX() {
        tornadoRoot.localScale = baseTornadoScale;
        tornadoRoot.position = new(tornadoRoot.position.x, tornadoStartHeight, tornadoRoot.position.z);

        for (int i = 0; i < particlesToColor.Length; i++) {
            ParticleSystem.MainModule main = particlesToColor[i].main;
            ParticleSystem.MinMaxGradient gradient = main.startColor;
            gradient.color = initialColorModules[i];
            main.startColor = gradient;
        }

        for (int i = 0; i < particlesToRotate.Length; i++) {
            ParticleSystem.RotationOverLifetimeModule rotation = particlesToRotate[i].rotationOverLifetime;
            rotation.y = initialYRotationModules[i];
        }

        foreach (ParticleSystemStopGroup group in particleStopGroups) {
            foreach (ParticleSystem ps in group.particles) {
                ParticleSystem.EmissionModule emission = ps.emission;
                emission.enabled = true;
            }
        }
    }
}
