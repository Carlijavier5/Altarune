using UnityEngine;

public partial class Player {
    private class PlayerLocomotionDriver {

        private LocomotionProperties lp;

        private readonly Player player;
        private CharacterController Controller => player.controller;

        public float MoveSpeed { get; private set; }
        public Vector3 MoveDir { get; private set; }

        private float verticalSpeed;
        private float alignmentMult;

        public PlayerLocomotionDriver(Player player) {
            this.player = player;
            MoveDir = Controller.transform.forward;
        }

        public void Move(Vector3 inputVector) {
            bool isMoving = inputVector.magnitude > 0;

            if (isMoving) {
                MoveSpeed = Mathf.MoveTowards(MoveSpeed, lp.maxSpeed, lp.linearAcceleration * Time.deltaTime);
                MoveDir = inputVector;
            } else {
                MoveSpeed = Mathf.MoveTowards(MoveSpeed, 0, lp.linearDrag * Time.deltaTime);
            }

            Vector3 moveVector = alignmentMult * MoveSpeed * MoveDir;
            Controller.Move(moveVector * player.FixedDeltaTime);
        }

        public void ResolveRotation() {
            Quaternion targetRotation = Quaternion.LookRotation(MoveDir, Vector3.up);
            Controller.transform.rotation = Quaternion.RotateTowards(Controller.transform.rotation, 
                                                                     targetRotation, lp.angularSpeed * Time.deltaTime);
            alignmentMult = Mathf.Clamp(1 - Quaternion.Angle(Controller.transform.rotation, targetRotation) / 180, 0, 1);
        }

        public void ResolveGravity() {
            player.IsGrounded = Controller.isGrounded;
            if (!player.IsGrounded) {
                verticalSpeed = Mathf.MoveTowards(verticalSpeed, -0.4f, player.DeltaTime / 2f);
                Controller.Move(new Vector3(0, verticalSpeed, 0));
            } else verticalSpeed = 0;
        }

        public void ReplaceProperties(LocomotionProperties lp) => this.lp = lp;
    }
}
