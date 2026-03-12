using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LockdownJoint<T> : MonoBehaviour {
    protected enum AxisLock { Free, Locked }

    [SerializeField] protected T follower, target;
    [SerializeField] protected AxisLock xPosition, yPosition, zPosition,
                                        xRotation, yRotation, zRotation;
    [SerializeField] protected float xROffset, yROffset, zROffset;
    [SerializeField] protected bool detachOnAwake;
    [SerializeField] protected bool playOnAwake = true;

    void Awake() {
        if (detachOnAwake) {
            transform.SetParent(null);
        }
        enabled = playOnAwake;
    }

    protected abstract Vector3 FollowerPosition { get; }
    protected abstract Quaternion FollowerRotation { get; }

    protected abstract Vector3 TargetPosition { get; }
    protected abstract Quaternion TargetRotation { get; }

    protected Vector3 Position => new(IsLocked(xPosition) ? TargetPosition.x : FollowerPosition.x,
                                      IsLocked(yPosition) ? TargetPosition.y : FollowerPosition.y,
                                      IsLocked(zPosition) ? TargetPosition.z : FollowerPosition.z);
    protected Quaternion Rotation => Quaternion.Euler(new Vector3((IsLocked(xRotation) ? TargetRotation.eulerAngles.x : FollowerRotation.eulerAngles.x) + xROffset,
                                                                  (IsLocked(yRotation) ? TargetRotation.eulerAngles.y : FollowerRotation.eulerAngles.y) + yROffset,
                                                                  (IsLocked(zRotation) ? TargetRotation.eulerAngles.z : FollowerRotation.eulerAngles.z) + zROffset));

    public void Play() {
        enabled = true;
    }

    public void Stop() {
        enabled = false;
    }

    protected bool IsLocked(AxisLock axisLock) {
        return axisLock == AxisLock.Locked;
    }
}