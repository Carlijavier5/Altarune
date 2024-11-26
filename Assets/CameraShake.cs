using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    private CinemachineVirtualCamera vCam;
    private float timer;

    private void Awake() {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void DoCameraShake(float intensity, float duration) {
        CinemachineBasicMultiChannelPerlin perlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        DOTween.To(() => perlin.m_AmplitudeGain, x => perlin.m_AmplitudeGain = x, intensity, duration / 2);
        StartCoroutine(KillTask(perlin, duration));
    }

    private IEnumerator KillTask(CinemachineBasicMultiChannelPerlin perlin, float duration) {
        yield return new WaitForSeconds(duration);
        DOTween.To(() => perlin.m_AmplitudeGain, x => perlin.m_AmplitudeGain = x, 0f, duration / 2);
    }
}
