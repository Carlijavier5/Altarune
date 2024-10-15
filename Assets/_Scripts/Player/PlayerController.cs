using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour {

    public event System.Action OnPlayerInit;
    public event System.Action OnDodgePerformed;
    public event System.Action OnMeleePerformed;

    [SerializeField] private CinemachineBrain cameraBrain;
    private PlayerInput playerInput;

    public Vector3 InputVector {
        get {
            Transform cameraTransform = cameraBrain ? cameraBrain.OutputCamera.transform 
                                                    : Camera.main.transform;

            Vector3 inputVector = playerInput.Movement.Movement.ReadValue<Vector2>();
            if (inputVector.magnitude > 0) {
                Vector3 camRight = cameraTransform.right,
                camForward = cameraTransform.forward;
                camRight.y = 0;
                camForward.y = 0;
                camRight.Normalize();
                camForward.Normalize();
                inputVector = (inputVector.x * camRight + inputVector.y * camForward).normalized;
            }
            return inputVector;
        }
        
    }

    void Awake() {
        playerInput = new();
        playerInput.Movement.Enable();
        playerInput.Actions.Enable();

        playerInput.Movement.Dodge.performed += Dodge_Performed;
        playerInput.Actions.MeleeAttack.performed += MeleeAttack_Performed;
        
        /// Replace after actual initialization;
        Init(cameraBrain);
    }

    public void Init(CinemachineBrain cameraBrain) {
        this.cameraBrain = cameraBrain;
        StartCoroutine(ISyncInitialization());
    }

    private IEnumerator ISyncInitialization() {
        yield return new WaitForEndOfFrame();
        OnPlayerInit?.Invoke();
    }

    private void Dodge_Performed(InputAction.CallbackContext context) {
        if (context.performed) OnDodgePerformed?.Invoke();
    }

    private void MeleeAttack_Performed(InputAction.CallbackContext context) {
        if (context.performed) OnMeleePerformed?.Invoke();
    }
}
