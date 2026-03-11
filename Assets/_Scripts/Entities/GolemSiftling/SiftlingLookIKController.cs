using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiftlingLookIKController : MonoBehaviour
{
    [SerializeField] private Transform ikBone;
    [Tooltip("The min is reached when the siftling looks forward, max when the max Y rotation is achieved;")]
    [SerializeField] private Vector2 xRotationRange;
    [SerializeField] private float maxYRotation;
    [SerializeField] private float baseZRotation;
    [SerializeField] private float lookSpeed;

    private Transform target;
    private float signedLerp;
    private Quaternion effectiveRotation;

    void Awake() {
        enabled = false;
    }

    public void Play(Transform target) {
        this.target = target;
        effectiveRotation = ikBone.localRotation;
        enabled = true;
    }

    public void Stop() {
        enabled = false;
    }

    void LateUpdate() {
        if (target) {
            Vector3 direction = new(ikBone.position.x - target.position.x, 0,
                                    ikBone.position.z - target.position.z);
            float angle = Vector3.SignedAngle(ikBone.forward, direction, Vector3.up);
            signedLerp = Mathf.Clamp(angle / maxYRotation, -1f, 1f);

            Quaternion rotationTarget = Quaternion.Euler(Mathf.Lerp(xRotationRange.x, xRotationRange.y, Mathf.Abs(signedLerp)),
                                                         Mathf.Lerp(-maxYRotation, maxYRotation, signedLerp * 0.5f + 0.5f),
                                                         baseZRotation);
            effectiveRotation = Quaternion.RotateTowards(effectiveRotation, rotationTarget, Time.deltaTime * lookSpeed);
            ikBone.localRotation = effectiveRotation;
        }
    }
}
