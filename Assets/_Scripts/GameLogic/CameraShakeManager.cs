using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour {

    private CinemachineBrain mainCam;
    private CinemachineBrain MainCam {
        get {
            if (mainCam == null) {
                Camera.main.TryGetComponent(out mainCam);
            }
            return mainCam;
        }
    }

    private CinemachineBasicMultiChannelPerlin perlinChannel;
    private CinemachineBasicMultiChannelPerlin PerlinChannel {
        get {
            if (MainCam != null && perlinChannel == null) {
                CinemachineCamera vCam = MainCam.ActiveVirtualCamera as CinemachineCamera;
                perlinChannel = vCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            }
            return perlinChannel;
        }
    }

    [SerializeField] private float baseIntensity = 3f;
    [SerializeField] private float baseDuration = 0.1f;

    private void Awake() {
        Camera.main.TryGetComponent(out mainCam);
    }
    
    public void DoCameraShake() {
        if (!PerlinChannel) return;
        PerlinChannel.AmplitudeGain = baseIntensity;
        StartCoroutine(KillTask(perlinChannel, baseDuration));
    }

    public void DoCameraShake(float intensity, float duration) {
        if (!PerlinChannel) return;
        DOTween.To(() => perlinChannel ? perlinChannel.AmplitudeGain : 0,
                    x => { if (perlinChannel) { perlinChannel.AmplitudeGain = x; } },
                    intensity, duration / 2 );
        StartCoroutine(KillTask(perlinChannel, duration));
    }

    private IEnumerator KillTask(CinemachineBasicMultiChannelPerlin perlin, float duration) {
        yield return new WaitForSeconds(duration);
        DOTween.To(() => perlin ? perlin.AmplitudeGain : 0,
                    x => { if (perlin) { perlin.AmplitudeGain = x; } },
                    0f, duration / 2);
    }
}