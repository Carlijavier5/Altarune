using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : ObjectModule {

    [HideInInspector][SerializeField] private MotionDriver defaultDriver;
    private readonly HashSet<PushActionCore> actionCores = new();
    private readonly Stack<PushActionCore> terminateStack = new();

    void Awake() {
        baseObject.OnTryFramePush += BaseObject_OnTryFramePush;
        baseObject.OnTryLongPush += BaseObject_OnTryLongPush;

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

    void FixedUpdate() {
        foreach (PushActionCore core in actionCores) {
            float lifetime = core.UpdateLifetime(Time.fixedDeltaTime);
            if (lifetime >= 1) terminateStack.Push(core);
            DoFramePush(core.direction);
        }

        while (terminateStack.TryPop(out PushActionCore core)) RemoveCore(core);
    }

    private void BaseObject_OnTryFramePush(Vector3 direction, EventResponse response) {
        DoFramePush(direction);
        response.received = true;
    }

    private void BaseObject_OnTryLongPush(Vector3 direction, float duration, LongPushResponse response) {
        PushActionCore core = new(this, direction, duration);
        actionCores.Add(core);
        response.actionCore = core;
    }

    private void DoFramePush(Vector3 direction) {
        switch (defaultDriver.MotionMode) {
            case MotionMode.Transform:
                defaultDriver.Transform.Translate(direction);
                break;
            case MotionMode.Rigidbody:
                Vector3 targetPosition = defaultDriver.Rigidbody.position + direction;
                defaultDriver.Rigidbody.MovePosition(targetPosition);
                break;
            case MotionMode.Controller:
                defaultDriver.Controller.Move(direction);
                break;
            case MotionMode.NavMesh:
                defaultDriver.NavMeshAgent.Move(direction);
                break;
        }
    }

    public void RemoveCore(PushActionCore core) => actionCores.Remove(core);

    #if UNITY_EDITOR
    public MotionDriver EDITOR_ONLY_DefaultDriver { get => defaultDriver;
                                                    set => defaultDriver = value; }
    #endif
}