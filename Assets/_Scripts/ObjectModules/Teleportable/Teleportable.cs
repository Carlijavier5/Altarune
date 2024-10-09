using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Teleportable : ObjectModule {

    [HideInInspector] [SerializeField] private MotionDriver defaultDriver = new();
    [HideInInspector] [SerializeField] private Pushable pushableModule;

    [SerializeField] private TeleportableProperties teleportableProperties;
    private TeleportableProperties TProps => teleportableProperties;

    private Vector3 fallbackScale;
    private float timer;

    void Awake() {
        baseObject.UpdateRendererRefs();
        baseObject.OnTryTeleport += BaseObject_OnTryTeleport;

        fallbackScale = TProps.rootTransform.localScale;

        if (pushableModule == null) {
            switch (defaultDriver.MotionMode) {
                case MotionMode.Transform:
                    baseObject.MotionDriver.Set(defaultDriver.Transform);
                    break;
                case MotionMode.Rigidbody:
                    baseObject.MotionDriver.Set(defaultDriver.Rigidbody);
                    break;
                case MotionMode.Controller:
                    baseObject.MotionDriver.Set(defaultDriver.Controller);
                    break;
                case MotionMode.NavMesh:
                    baseObject.MotionDriver.Set(defaultDriver.NavMeshAgent);
                    break;
            }
        }
    }

    private void BaseObject_OnTryTeleport(Vector3 desiredPosition, EventResponse<Vector3> response) {
        if (timer == 0 && NavMesh.SamplePosition(desiredPosition, out NavMeshHit hitInfo, 4, NavMesh.AllAreas)) {
            Debug.Log(TProps.duration);
            StopAllCoroutines();
            StartCoroutine(ITeleport(hitInfo.position));
            response.objectReference = desiredPosition;
            response.received = true;
        }
    }

    private void UpdateRootScale(float lerpValue, Vector3 originalScale) {
        Vector3 targetScale = new Vector3(TProps.settings.scaleCurveXZ.Evaluate(lerpValue),
                                          TProps.settings.scaleCurveY.Evaluate(lerpValue),
                                          TProps.settings.scaleCurveXZ.Evaluate(lerpValue));
        TProps.rootTransform.localScale = Vector3.Scale(targetScale, originalScale);
    }

    private IEnumerator ITeleport(Vector3 targetPosition) {
        baseObject.SetMaterial(TProps.settings.material);
        Vector3 originalScale = TProps.rootTransform.localScale;

        while (timer < TProps.duration) {
            timer = Mathf.MoveTowards(timer, TProps.duration, Time.deltaTime);
            if (baseObject.MotionDriver.MotionMode != MotionMode.NavMesh) {
                UpdateRootScale(timer / TProps.duration, originalScale);
            }
            yield return null;
        }

        yield return new WaitForFixedUpdate();
        MotionDriver driver = baseObject.MotionDriver;
        switch (driver.MotionMode) {
            case MotionMode.Transform:
            case MotionMode.Rigidbody:
                driver.Transform.position = targetPosition;
                break;
            case MotionMode.Controller:
                driver.Controller.enabled = false;
                driver.Transform.position = targetPosition;
                driver.Controller.enabled = true;
                break;
            case MotionMode.NavMesh:
                driver.NavMeshAgent.Warp(targetPosition);
                driver.NavMeshAgent.enabled = false;
                yield return new WaitForFixedUpdate();
                driver.NavMeshAgent.enabled = true;
                break;
        }


        while (timer > 0) {
            timer = Mathf.MoveTowards(timer, 0, Time.deltaTime);
            if (baseObject.MotionDriver.MotionMode != MotionMode.NavMesh) {
                UpdateRootScale(timer / TProps.duration, originalScale);
            }
            yield return null;
        }

        baseObject.ResetMaterials();
    }

    void OnDisable() {
        if (baseObject) {
            baseObject.ResetMaterials();
            TProps.rootTransform.localScale = fallbackScale;
            timer = 0;
        }
    }


    #if UNITY_EDITOR
    public MotionDriver EDITOR_ONLY_DefaultDriver {
        get => defaultDriver;
        set => defaultDriver = value;
    }

    public bool HasPushable => pushableModule != null;

    protected override void Reset() {
        base.Reset();
        defaultDriver.Set(transform);
        if (CJUtils.AssetUtils.TryRetrieveAsset(out DefaultTeleportableSettings settings)) {
            teleportableProperties = new(settings, transform);
        }
    }

    public override void EDITOR_ONLY_AttachModule() { 
        if (pushableModule == null) TryGetComponent(out pushableModule);
    }
    #endif
}