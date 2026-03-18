using UnityEngine;

public class LockdownJointTransform : LockdownJoint<Transform> {

    protected override Vector3 FollowerPosition => follower.position;
    protected override Quaternion FollowerRotation => follower.rotation;

    protected override Vector3 TargetPosition => target.position;
    protected override Quaternion TargetRotation => target.rotation;

    void Update() {
        Apply();
    }

    public override void Apply() {
        follower.SetPositionAndRotation(Position, Rotation);
    }
}