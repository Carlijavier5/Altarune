using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private float linearAcceleration,
                                   linearDrag, angularSpeed,
                                   maxSpeed;

    private PlayerInput playerInput;
    private float moveSpeed, alignmentMult;
    private Vector3 moveVector, moveDir;

    void Awake() {
        playerInput = new();
        playerInput.Movement.Enable();
        moveDir = transform.forward;
    }

    void FixedUpdate() {
        FloorMovement();
        FloorRotation();
        controller.Move(moveVector * Time.deltaTime);
    }

    private void FloorMovement() {
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

    private void FloorRotation() {
        Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);
        alignmentMult = Mathf.Clamp(1 - Quaternion.Angle(transform.rotation, targetRotation) / 180, 0, 1);
    }
}
