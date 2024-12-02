using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class SavageCutsceneManager : MonoBehaviour {

    private const string SPIN_START_PARAM = "SpinStart";

    public event System.Action OnCutsceneEnd;

    [SerializeField] private Animator cutsceneAnimator;
    [SerializeField] private Material savageDecal;
    [SerializeField] private new Light light;

    [SerializeField] private float spinDelay = 2f;
    [SerializeField] private float wireDelay = 1.5f;
    [SerializeField] private float layerShiftTime;

    [SerializeField] private List<Rigidbody> wireConnectors;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private ParticleSystem bigDust;

    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private Transform uiTop;
    [SerializeField] private Transform uiBottom;
    [SerializeField] private Light mainLight;
    [SerializeField] private List<Transform> fires;
    [SerializeField] private List<Light> fireLights;
    [SerializeField] private List<Transform> sand;

    void Awake() {
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

    public void Activate() {
        StartCoroutine(Cutscene());
    }

    private IEnumerator Cutscene() {
        GM.CameraShakeManager.DoCameraShake(10, 3f);
        dust.Play();
        foreach (Transform sands in sand) {
            sands.DOScaleY(100f, 0.5f);
        }
        StartCoroutine(Decal());
        DOTween.To(() => light.intensity, x => light.intensity = x, 100, 2f);
        yield return new WaitForSeconds(spinDelay);
        cutsceneAnimator.SetTrigger(SPIN_START_PARAM);
        yield return new WaitForSeconds(wireDelay);
        foreach (Rigidbody rigidbody in wireConnectors) {
            rigidbody.useGravity = true;
        }

        yield return new WaitForSeconds(0.8f);
        StartCoroutine(UI());
        vCam.m_Priority = 100;
        bigDust.Play();
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(2.5f);
        Time.timeScale = 1f;
        vCam.m_Priority = 0;
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
        yield return new WaitForSeconds(1f);
        float lerpVal, timer = 0;
        while (timer < layerShiftTime) {
            timer = Mathf.MoveTowards(timer, layerShiftTime, Time.unscaledDeltaTime);
            lerpVal = timer / layerShiftTime;
            cutsceneAnimator.SetLayerWeight(0, 1 - lerpVal);
            cutsceneAnimator.SetLayerWeight(1, lerpVal);
            yield return null;
        }
        OnCutsceneEnd?.Invoke();
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

    void OnDisable() {
        savageDecal.SetFloat("_Lerp", 0f);
    }
}
