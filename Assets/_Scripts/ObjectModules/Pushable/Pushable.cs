using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : ObjectModule {

    [HideInInspector][SerializeField] private MotionDriver defaultDriver;

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

    private void BaseObject_OnTryLongPush(Vector3 arg1, float arg2, EventResponse arg3) {
        throw new System.NotImplementedException();
    }

    private void BaseObject_OnTryFramePush(Vector3 arg1, EventResponse arg2) {
        throw new System.NotImplementedException();
    }
}
