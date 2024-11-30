using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : ObjectModule {

    [HideInInspector][SerializeField] private MotionDriver defaultDriver = new();
    [HideInInspector][SerializeField] private CrowdControllable ccModule;

    [SerializeField] private PushableAttributes pushableProperties;

    private RuntimePushAttributes runtimeProperties;

    private Vector3 dynamicVelocityAdjustment;
    private Vector3 DynamicVelocityAdjustment {
        get => dynamicVelocityAdjustment;
        set {
            MotionDriver driver = baseObject.MotionDriver;
            if (driver.Rigidbody && !driver.Rigidbody.isKinematic) {
                baseObject.MotionDriver.Rigidbody.velocity += value - dynamicVelocityAdjustment;
                dynamicVelocityAdjustment = value;
            }
        }
    }

    private readonly Queue<Vector3> pastImpulseQueue = new();

    private readonly HashSet<PushActionCore> actionCores = new();
    private readonly Stack<PushActionCore> terminateStack = new();

    void Awake() {
        baseObject.OnTryFramePush += BaseObject_OnTryFramePush;
        baseObject.OnTryLongPush += BaseObject_OnTryLongPush;
        baseObject.MotionDriver.OnModeChange += MotionDriver_OnModeChange;

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

        IEnumerable<EntityEffect> effectSource = baseObject is Entity ? (baseObject as Entity).StatusEffects
                                                                      : null;
        runtimeProperties = pushableProperties.RuntimeClone(effectSource);
    }

    private void MotionDriver_OnModeChange() {
        pastImpulseQueue.Clear();
        dynamicVelocityAdjustment = Vector3.zero;
    }

    void FixedUpdate() {
        foreach (PushActionCore core in actionCores) {
            float lifetime = core.UpdateLifetime(Time.fixedDeltaTime);
            if (lifetime >= 1) terminateStack.Push(core);
            DoFramePush(core.CurrentPushVector);
        }

        while (terminateStack.TryPop(out PushActionCore core)) core.Kill();
    }

    private void BaseObject_OnTryFramePush(Vector3 direction, EventResponse response) {
        DoFramePush(direction);
        response.received = true;
    }

    private void BaseObject_OnTryLongPush(Vector3 direction, float duration, 
                                          EventResponse<PushActionCore> response) {
        PushActionCore core = new(this, direction, duration, pushableProperties.easeCurves);
        actionCores.Add(core);
        response.objectReference = core;
        response.received = true;
    }

    private void DoFramePush(Vector3 direction) {
        direction = runtimeProperties.ComputePush(direction);
        MotionDriver driver = baseObject.MotionDriver;
        switch (driver.MotionMode) {
            case MotionMode.Transform:
                driver.Transform.Translate(direction * Time.fixedDeltaTime);
                break;
            case MotionMode.Rigidbody:
                if (driver.Rigidbody.isKinematic) {
                    Vector3 targetPosition = driver.Rigidbody.position
                                           + direction * Time.fixedDeltaTime;
                    driver.Rigidbody.MovePosition(targetPosition);
                } else {
                    while (pastImpulseQueue.TryDequeue(out Vector3 impulse)) {
                        DynamicVelocityAdjustment -= impulse;
                    } DynamicVelocityAdjustment += direction;
                    pastImpulseQueue.Enqueue(direction);
                } break;
            case MotionMode.Controller:
                if (driver.Controller.enabled) {
                    driver.Controller.Move(direction * Time.fixedDeltaTime);
                } break;
            case MotionMode.NavMesh:
                if (driver.NavMeshAgent.isActiveAndEnabled) {
                    driver.NavMeshAgent.Move(direction * Time.fixedDeltaTime);
                } break;
        }
    }

    public void RemoveCore(PushActionCore core) => actionCores.Remove(core);

    #if UNITY_EDITOR
    public MotionDriver EDITOR_ONLY_DefaultDriver { get => defaultDriver;
                                                    set => defaultDriver = value; }

    protected override void Reset() {
        base.Reset();
        defaultDriver.Set(transform);
        if (CJUtils.AssetUtils.TryRetrieveAsset(out DefaultEaseCurves curves)) {
            pushableProperties = new(curves);
        }
    }

    public override void EDITOR_ONLY_AttachModule() {
        if (ccModule == null) TryGetComponent(out ccModule);
    }
    #endif
}