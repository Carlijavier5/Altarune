using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour {

    private CinemachineBrain mainCam;
    private CinemachineBrain MainCam {
        get {
            if (mainCam == null) {
                TryGetComponent(out mainCam);
            } return mainCam;
        }
    }

    private CinemachineVirtualCamera vCam;

    [SerializeField] private float baseIntensity = 3f;
    [SerializeField] private float baseDuration = 0.1f;

    private void Awake() {
        Camera.main.TryGetComponent(out mainCam);
    }
    
    public void DoCameraShake() {
        if (MainCam == null) return;
        vCam = MainCam.ActiveVirtualCamera as CinemachineVirtualCamera;
        CinemachineBasicMultiChannelPerlin perlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (!perlin) return;
        perlin.m_AmplitudeGain = baseIntensity;
        StartCoroutine(KillTask(perlin, baseDuration));
    }

    public void DoCameraShake(float intensity, float duration) {
        if (MainCam == null) return;
        vCam = MainCam.ActiveVirtualCamera as CinemachineVirtualCamera;
        CinemachineBasicMultiChannelPerlin perlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (!perlin) return;
        DOTween.To(() => perlin.m_AmplitudeGain, x => perlin.m_AmplitudeGain = x, intensity, duration / 2);
        StartCoroutine(KillTask(perlin, duration));
    }

    private IEnumerator KillTask(CinemachineBasicMultiChannelPerlin perlin, float duration) {
        yield return new WaitForSeconds(duration);
        DOTween.To(() => perlin.m_AmplitudeGain, x => perlin.m_AmplitudeGain = x, 0f, duration / 2);
    }
}
