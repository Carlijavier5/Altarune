using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    private static CameraShake instance;
    public static CameraShake Instance => instance;

    private CinemachineBrain mainCam;
    private CinemachineVirtualCamera vCam;
    private float timer;

    [SerializeField] private float baseIntensity = 3f;
    [SerializeField] private float baseDuration = 0.1f;

    private void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else instance = this;
        
        mainCam = Camera.main.GetComponent<CinemachineBrain>();
    }
    
    public void DoCameraShake() {
        vCam = mainCam.ActiveVirtualCamera as CinemachineVirtualCamera;
        CinemachineBasicMultiChannelPerlin perlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = baseIntensity;
        StartCoroutine(KillTask(perlin, baseDuration));
    }

    public void DoCameraShake(float intensity, float duration) {
        vCam = mainCam.ActiveVirtualCamera as CinemachineVirtualCamera;
        CinemachineBasicMultiChannelPerlin perlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        DOTween.To(() => perlin.m_AmplitudeGain, x => perlin.m_AmplitudeGain = x, intensity, duration / 2);
        StartCoroutine(KillTask(perlin, duration));
    }

    private IEnumerator KillTask(CinemachineBasicMultiChannelPerlin perlin, float duration) {
        yield return new WaitForSeconds(duration);
        DOTween.To(() => perlin.m_AmplitudeGain, x => perlin.m_AmplitudeGain = x, 0f, duration / 2);
    }
}
