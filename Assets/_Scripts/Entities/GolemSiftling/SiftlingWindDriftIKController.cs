using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SiftlingWindDriftIKController : MonoBehaviour
{
    private enum State { Idle, Drifting };
    private State state;

    [SerializeField] private Transform rootIKBone, crystalIKBone;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [Tooltip("Remaps the linear speed of the siftling in this range to the crystal's X drift;")]
    [SerializeField] private Vector2 linearSpeedRange;
    [SerializeField] private Vector2 forwardCrystalDriftRange;
    [Tooltip("Remaps the perceived angular speed of the siftling in this range to the root's Y drift and the crystal's Y drift;")]
    [SerializeField] private Vector2 angularSpeedRange;
    [SerializeField] private Vector2 rightCrystalDriftRange;
    [SerializeField] private Vector2 rootDriftRange;
    [Tooltip("The speed at which the bone rotation is adjusted, to prevent snapping;")]
    [SerializeField] private float boneAdjustmentTime;
    [SerializeField] private float restorationAdjustmentMultiplier;

    private float forwardCrystalDriftAngle, rightCrystalDriftAngle, rootDriftAngle;
    private Quaternion crystalDriftRotation, rootDriftRotation;
    private float forwardCrystalDriftVelocity, rightCrystalDriftVelocity, rootDriftVelocity;

    private float crystalDriftLength, rootDriftLength;

    private float prevRootEulerY;

    void Awake() {
        linearSpeedRange.Scale(linearSpeedRange);
        crystalDriftLength = Mathf.Abs(forwardCrystalDriftRange.x - forwardCrystalDriftRange.y);
        rootDriftLength = Mathf.Abs(rootDriftRange.x - rootDriftRange.y);
        enabled = false;
    }

    public void TryPlay() {
        if (state != State.Drifting) {
            forwardCrystalDriftAngle = crystalIKBone.localEulerAngles.x;
            crystalDriftRotation = crystalIKBone.localRotation;

            rootDriftAngle = rootIKBone.localEulerAngles.z;
            rootDriftRotation = rootIKBone.localRotation;

            prevRootEulerY = navMeshAgent.transform.eulerAngles.y;

            enabled = true;
            state = State.Drifting;
        }
    }

    public void Stop() {
        state = State.Idle;
    }

    void LateUpdate() {
        switch (state) {
            case State.Drifting:
                float rootAngleDelta = Mathf.DeltaAngle(prevRootEulerY, navMeshAgent.transform.eulerAngles.y) / Time.deltaTime;
                float angularLerpVal = Mathf.InverseLerp(angularSpeedRange.x, angularSpeedRange.y, rootAngleDelta);

                float linearLerpVal = Mathf.InverseLerp(linearSpeedRange.x, linearSpeedRange.y, navMeshAgent.velocity.sqrMagnitude);
                float targetXAngle = Mathf.Lerp(forwardCrystalDriftRange.x, forwardCrystalDriftRange.y, linearLerpVal);
                float targetYAngle = Mathf.Lerp(rightCrystalDriftRange.x, rightCrystalDriftRange.y, angularLerpVal);
                forwardCrystalDriftAngle = Mathf.SmoothDampAngle(forwardCrystalDriftAngle, targetXAngle, ref forwardCrystalDriftVelocity, boneAdjustmentTime);
                rightCrystalDriftAngle = Mathf.SmoothDampAngle(rightCrystalDriftAngle, targetYAngle, ref rightCrystalDriftVelocity, boneAdjustmentTime);
                DoCrystalDrift(Quaternion.Euler(forwardCrystalDriftAngle, rightCrystalDriftAngle, -rightCrystalDriftAngle));

                targetYAngle = Mathf.Lerp(rootDriftRange.x, rootDriftRange.y, angularLerpVal);
                rootDriftAngle = Mathf.SmoothDampAngle(rootDriftAngle, targetYAngle, ref rootDriftVelocity, boneAdjustmentTime);
                DoRootDrift(Quaternion.Euler(rootIKBone.localEulerAngles.x, rootIKBone.localEulerAngles.y, rootDriftAngle));

                prevRootEulerY = navMeshAgent.transform.eulerAngles.y;
                break;
            case State.Idle:
                Quaternion crystalTarget = crystalIKBone.localRotation;
                Quaternion rootTarget = rootIKBone.localRotation;
                DoCrystalDrift(crystalIKBone.localRotation);
                DoRootDrift(rootIKBone.localRotation);
                if (crystalDriftRotation == crystalTarget
                        && rootDriftRotation == rootTarget) {
                    enabled = false;
                }
                break;
        }
    }

    private void DoBoneDrift(Transform targetBone, ref Quaternion effectiveRotation, Quaternion targetRotation, float driftLength) {
        effectiveRotation = Quaternion.RotateTowards(effectiveRotation, targetRotation,
                                                    (Time.deltaTime * driftLength
                                                     * state switch {State.Idle => restorationAdjustmentMultiplier, _ => 1f }
                                                     ).SafeDivide(boneAdjustmentTime));
        targetBone.localRotation = effectiveRotation;
    }

    private void DoRootDrift(Quaternion targetRotation) {
        DoBoneDrift(rootIKBone, ref rootDriftRotation, targetRotation, rootDriftLength);
    }

    private void DoCrystalDrift(Quaternion targetRotation) {
        DoBoneDrift(crystalIKBone, ref crystalDriftRotation, targetRotation, crystalDriftLength);
    }
}
