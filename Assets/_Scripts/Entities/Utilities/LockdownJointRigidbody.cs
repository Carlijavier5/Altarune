using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockdownJointRigidbody : LockdownJoint<Rigidbody>
{
    protected override Vector3 FollowerPosition => follower.position;
    protected override Quaternion FollowerRotation => follower.rotation;

    protected override Vector3 TargetPosition => target.position;
    protected override Quaternion TargetRotation => target.rotation;

    void FixedUpdate() {
        follower.position = Position;
        follower.rotation = Rotation;
    }
}
