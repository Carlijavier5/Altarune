using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockdownJoint : MonoBehaviour
{
    private enum AxisLock { Free, Locked }

    [SerializeField] private Rigidbody followerRB, targetRB;
    [SerializeField] private AxisLock xPosition, yPosition, zPosition,
                                      xRotation, yRotation, zRotation;
    [SerializeField] private float xROffset, yROffset, zROffset;

    public void Play() {
        enabled = true;
    }

    public void Stop() {
        enabled = false;
    }

    void FixedUpdate() {
        followerRB.position = new Vector3(IsLocked(xPosition) ? targetRB.position.x : followerRB.position.x,
                                          IsLocked(yPosition) ? targetRB.position.y : followerRB.position.y,
                                          IsLocked(zPosition) ? targetRB.position.z : followerRB.position.z);
        followerRB.rotation = Quaternion.Euler(new Vector3((IsLocked(xRotation) ? targetRB.rotation.eulerAngles.x : followerRB.rotation.eulerAngles.x) + xROffset,
                                                           (IsLocked(yRotation) ? targetRB.rotation.eulerAngles.y : followerRB.rotation.eulerAngles.y) + yROffset,
                                                           (IsLocked(zRotation) ? targetRB.rotation.eulerAngles.z : followerRB.rotation.eulerAngles.z) + zROffset));
    }

    private bool IsLocked(AxisLock axisLock) {
        return axisLock == AxisLock.Locked;
    }
}
