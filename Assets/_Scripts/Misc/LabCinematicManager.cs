using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class LabCinematicManager : CCondition {
    [SerializeField] private CCondition condition;
    [SerializeField] private CinemachineVirtualCamera cinematicCamera;
    [SerializeField] private float cameraShakeIntensity = 2f;
    [SerializeField] private Vector3 initOffset;
    [SerializeField] private VisualEffect rippleEffect;
    [SerializeField] private Transform room;
    [SerializeField] private SFXManagedLoop roomQuakeSFX, waterfallSFX;
    [SerializeField] private SFXOneShot stairRise, pillarRise;
    private Vector3 roomPos;

    [SerializeField] private List<Transform> pillars;
    private List<Vector3> pillarInitPos = new List<Vector3>();
    
    [SerializeField] private List<Transform> stairs;
    private List<Vector3> stairInitPos = new List<Vector3>();
    [SerializeField] private Transform stairbarrier;
    private Vector3 stairBarrierInitPos;

    [SerializeField] private Material barrier;
    [SerializeField] private List<Light> lights;
    private List<float> intensities = new List<float>();

    [SerializeField] private VisualEffect waterfallRipple1;
    [SerializeField] private VisualEffect waterfallRipple2;

    [Header("Delays")] [SerializeField] private float roomTime = 5f;
    [SerializeField] private float stairMoveTime = 2f;
    [SerializeField] private float stairStaggerTime = 0.2f;

    [SerializeField] private float lightTime = 2f;

    [SerializeField] private float waitTime = 2f;

    [SerializeField] private float initTime = 3f;
    // Start is called before the first frame update
    void Start() {
        condition.OnConditionTrigger += RunCinematicAction;
        rippleEffect.Stop();
        waterfallRipple1.Stop();
        waterfallRipple2.Stop();
        pillarInitPos = new List<Vector3>(pillars.Count);
        stairInitPos = new List<Vector3>(stairs.Count);
        intensities = new List<float>(intensities.Count);
        barrier.SetFloat("_Global_Alpha", 0f);
        roomPos = room.position;
        room.position += initOffset;
        for (int i = 0; i < pillars.Count; i++) {
            pillarInitPos.Add(pillars[i].position);
            pillars[i].position -= initOffset;
        }
        for (int i = 0; i < stairs.Count; i++) {
            stairInitPos.Add(stairs[i].position);
            stairs[i].position -= initOffset;
        }
        for (int i = 0; i < lights.Count; i++) {
            intensities.Add(lights[i].intensity);
            lights[i].intensity = 0;
        }

        stairBarrierInitPos = stairbarrier.position;
        stairbarrier.position -= initOffset;
    }

    public void RunCinematicAction(CConditionData data) {
        StartCoroutine(RunCinematic());
    }

    private IEnumerator RunCinematic() {
        yield return new WaitForSeconds(initTime);
        cinematicCamera.m_Priority = 100;
        yield return new WaitForSeconds(1f);
        rippleEffect.Play();
        GM.CameraShakeManager.DoCameraShake(cameraShakeIntensity, roomTime);
        roomQuakeSFX.Play();
        yield return new WaitForSeconds(1f);
        room.DOMove(roomPos, roomTime);
        StartCoroutine(AsyncFalls());
        waterfallSFX.Play();
        yield return new WaitForSeconds(roomTime * 0.7f);
        roomQuakeSFX.Stop(2);
        waterfallSFX.Stop(1.5f);
        stairbarrier.DOMove(stairBarrierInitPos, stairMoveTime);
        yield return new WaitForSeconds(stairStaggerTime);
        rippleEffect.Stop();
        StartCoroutine(AsyncPillar());
        for (int i = 0; i < stairs.Count; i++) {
            stairRise.Play();
            stairs[i].DOMove(stairInitPos[i], stairMoveTime).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(stairStaggerTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        for (int i = 0; i < lights.Count; i++) {
            Light light = lights[i];
            DOTween.To(() => light.intensity, x => light.intensity = x, intensities[i], lightTime);
        }

        barrier.DOFloat(0.5f, "_Global_Alpha", lightTime);
        yield return new WaitForSeconds(waitTime);
        CheckCondition();
        cinematicCamera.m_Priority = 0;
        yield return null;
    }

    private IEnumerator AsyncPillar() {
        yield return new WaitForSeconds(1f);
        for (int i = 1; i < pillars.Count; i+=2) {
            pillarRise.Play();
            pillars[i - 1].DOMove(pillarInitPos[i - 1], stairMoveTime * 1.4f).SetEase(Ease.OutCubic);
            pillars[i].DOMove(pillarInitPos[i], stairMoveTime * 1.4f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(stairStaggerTime * 2);
            yield return null;
        }
    }
    
    private IEnumerator AsyncFalls() {
        yield return new WaitForSeconds(2f);
        waterfallRipple1.Play();
        waterfallRipple2.Play();
    }
}
