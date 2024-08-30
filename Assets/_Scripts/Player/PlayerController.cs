using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private float linearAcceleration,
                                   linearDrag, angularSpeed,
                                   maxSpeed, dodgeSpeed;

    private PlayerInput playerInput;
    private float moveSpeed, alignmentMult;
    private Vector3 moveVector, moveDir;

    private class PHDodgeBuffer {
        public Vector3 dir;
        public float amount;
        public PHDodgeBuffer(Vector3 dir) => this.dir = dir;
        public bool Perform() => (amount = Mathf.MoveTowards(amount, 1, Time.deltaTime * 4.5f)) == 1;
    } private PHDodgeBuffer dodgeBuffer;

    void Awake() {
        playerInput = new();
        playerInput.Movement.Enable();
        playerInput.Movement.Dodge.performed += Dodge_performed;
        moveDir = transform.forward;
    }

    private void Dodge_performed(InputAction.CallbackContext context) {
        if (context.performed && dodgeBuffer == null) {
            Vector2 inputVector = playerInput.Movement.Movement.ReadValue<Vector2>();
            Vector3 dir = inputVector.magnitude > 0 ? new Vector3(inputVector.x, 0, inputVector.y) : moveDir;
            dodgeBuffer = new PHDodgeBuffer(dir);
            moveDir = dir;
            moveSpeed = 0;
            animator.Play("Dodge");
        }
    }

    void FixedUpdate() {
        if (dodgeBuffer == null) {
            InputMovement();
        } else {
            InputRoll();
        } InputRotation();
        controller.Move(moveVector * Time.deltaTime);
    }

    private void InputMovement() {
        Vector2 inputVector = playerInput.Movement.Movement.ReadValue<Vector2>();
        bool isMoving = inputVector.magnitude != 0;

        if (isMoving) {
            moveSpeed = Mathf.MoveTowards(moveSpeed, maxSpeed, linearAcceleration * Time.deltaTime);
            Vector3 camRight = Camera.main.transform.right,
                    camForward = Camera.main.transform.forward;
            camRight.y = 0;
            camForward.y = 0;
            camRight.Normalize();
            camForward.Normalize();
            moveDir = (inputVector.x * camRight + inputVector.y * camForward).normalized;
        } else {
            moveSpeed = Mathf.MoveTowards(moveSpeed, 0, linearDrag * Time.deltaTime);
        }

        moveVector = alignmentMult * moveSpeed * moveDir;
        animator.SetFloat("MoveSpeed", (moveSpeed / maxSpeed));
    }

    private void InputRoll() {
        if (dodgeBuffer.Perform()) {
            moveSpeed = moveSpeed = Mathf.MoveTowards(moveSpeed, 0, linearDrag * Time.deltaTime * 2);
            if (moveSpeed <= maxSpeed / 2) dodgeBuffer = null;
        } else {
            moveSpeed = Mathf.MoveTowards(moveSpeed, dodgeSpeed, linearAcceleration * 6.5f * Time.deltaTime);
        }
        moveVector = moveSpeed * moveDir;
    }

    private void InputRotation() {
        Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);
        alignmentMult = Mathf.Clamp(1 - Quaternion.Angle(transform.rotation, targetRotation) / 180, 0, 1);
    }
}
