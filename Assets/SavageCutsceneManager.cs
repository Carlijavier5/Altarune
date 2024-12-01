using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SavageCutsceneManager : MonoBehaviour {
    [SerializeField] private Animator cutsceneAnimator;
    [SerializeField] private Material savageDecal;
    [SerializeField] private Light light;

    [SerializeField] private float initDelay = 1f;
    [SerializeField] private float textDelay = 3f;
    [SerializeField] private float spinDelay = 2f;
    [SerializeField] private float wireDelay = 1.5f;

    [SerializeField] private List<Rigidbody> wireConnectors;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private ParticleSystem bigDust;

    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private Transform uiTop;
    [SerializeField] private Transform uiBottom;
    [SerializeField] private Light mainLight;
    [SerializeField] private List<Transform> fires;
    [SerializeField] private List<Light> fireLights;
    [SerializeField] private List<Transform> sand;

    private CameraShake shake;

    private void Start() {
        shake = GetComponent<CameraShake>();
        StartCoroutine(Cutscene());
        uiTop.position += (new Vector3(0f, 400f, 0f));
        uiBottom.position -= (new Vector3(0f, 600f, 0f));
        foreach (Transform fire in fires) {
            fire.DOScale(0f, 0f);
        }
        foreach (Light fire in fireLights) {
            fire.intensity = 0f;
        }
        foreach (Transform sands in sand) {
            sands.DOScaleY(0f, 0f);
        }
    }

    private IEnumerator Cutscene() {
        yield return new WaitForSeconds(initDelay);
        yield return new WaitForSeconds(textDelay);
        shake.DoCameraShake(10, 3f);
        dust.Play();
        foreach (Transform sands in sand) {
            sands.DOScaleY(100f, 0.5f);
        }
        StartCoroutine(Decal());
        DOTween.To(() => light.intensity, x => light.intensity = x, 100, 2f);
        yield return new WaitForSeconds(spinDelay);
        cutsceneAnimator.SetTrigger("Spin");
        yield return new WaitForSeconds(wireDelay);
        foreach (Rigidbody rigidbody in wireConnectors) {
            rigidbody.useGravity = true;
        }

        yield return new WaitForSeconds(0.8f);
        StartCoroutine(UI());
        camera.m_Priority = 100;
        bigDust.Play();
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(2.5f);
        Time.timeScale = 1f;
        camera.m_Priority = 0;
        foreach (Rigidbody rigidbody in wireConnectors) {
            rigidbody.useGravity = false;
        }
        DOTween.To(() => light.intensity, x => light.intensity = x, 0, 2f);
        DOTween.To(() => mainLight.intensity, x => mainLight.intensity = x, 0.15f, 2f);
        yield return new WaitForSeconds(1f);
        foreach (Transform fire in fires) {
            fire.DOScale(new Vector3(0.05f, 0.2f, 0.05f), 0.5f).SetEase(Ease.OutBack);
        }
        foreach (Light fire in fireLights) {
            DOTween.To(() => fire.intensity, x => fire.intensity = x, 2.26f, 0.5f).SetEase(Ease.OutBack);
        }
    }
    
    private IEnumerator Decal() {
        yield return new WaitForSeconds(1f);
        savageDecal.DOFloat(1f, "_Lerp", 2f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(3f);
        savageDecal.DOFloat(0f, "_Lerp", 0.5f);
    }
    
    private IEnumerator UI() {
        yield return new WaitForSeconds(0.5f);
        uiTop.DOMoveY(uiTop.position .y - 400f, 1f);
        uiBottom.DOMoveY(uiBottom.position.y + 600f, 1f);
        yield return new WaitForSeconds(2.5f);
        uiTop.DOMoveY(uiTop.position .y + 400f, 1.5f);
        uiBottom.DOMoveY(uiBottom.position.y - 600f, 1.5f);
    }
}
