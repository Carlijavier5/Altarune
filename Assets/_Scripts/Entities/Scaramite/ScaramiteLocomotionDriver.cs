using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Scaramite {
    public class ScaramiteLocomotionDriver {

        private readonly Scaramite scaramite;
        private CharacterController Controller => scaramite.controller;

        public float MoveSpeed { get; private set; }
        public Vector3 MoveDir { get; private set; }

        private readonly float linearAcceleration, linearDrag, angularSpeed;
        private float maxSpeed, alignmentMult;

        public ScaramiteLocomotionDriver(Scaramite scaramite, float linearAcceleration,
                                         float angularSpeed, float linearDrag) {
            this.scaramite = scaramite;
            this.linearAcceleration = linearAcceleration;
            this.angularSpeed = angularSpeed;
            this.linearDrag = linearDrag;
            MoveDir = Controller.transform.forward;
        }

        public void Move(Vector3 inputVector) {
            inputVector.y = 0;
            bool isMoving = inputVector.magnitude > 0;

            if (isMoving) {
                MoveSpeed = Mathf.MoveTowards(MoveSpeed, maxSpeed, linearAcceleration * scaramite.FixedDeltaTime);
                MoveDir = inputVector.normalized;
            } else {
                MoveSpeed = Mathf.MoveTowards(MoveSpeed, 0, linearDrag * scaramite.FixedDeltaTime);
            }

            Vector3 moveVector = alignmentMult * MoveSpeed * MoveDir;
            if (scaramite.CanMove) Controller.Move(scaramite.RootMult 
                                                   * scaramite.FixedDeltaTime
                                                   * moveVector);
        }

        public void SetMaxSpeed(float maxSpeed) => this.maxSpeed = maxSpeed; 

        public void LookAt(Vector3 lookTarget) {
            MoveDir = (lookTarget - scaramite.transform.position).normalized;
        }

        public void ResolveRotation() {
            if (MoveDir.magnitude > 0) {
                Quaternion targetRotation = Quaternion.LookRotation(MoveDir, Vector3.up);
                Controller.transform.rotation = Quaternion.RotateTowards(Controller.transform.rotation,
                                                                         targetRotation, angularSpeed * scaramite.FixedDeltaTime);
                alignmentMult = Mathf.Clamp(1 - Quaternion.Angle(Controller.transform.rotation, targetRotation) / 180, 0, 1);
            }
        }
    }
}
