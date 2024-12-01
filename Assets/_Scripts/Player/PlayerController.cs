using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour {

    public event System.Action OnPlayerInit;
    public event System.Action OnDodgePerformed;
    public event System.Action OnMeleePerformed;

    public event System.Action OnSummonPerformed;
    public event System.Action<SummonType, int> OnSummonSelect;

    public event System.Action OnSkillStarted;
    public event System.Action OnSkillCast;

    [SerializeField] private Transform cameraTarget;
    public Transform CameraTarget => cameraTarget;

    private CinemachineBrain cameraBrain;
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

    public Camera OutputCamera => cameraBrain ? cameraBrain.OutputCamera
                                              : Camera.main;

    public Vector2 CursorPosition => Mouse.current.position.ReadValue();

    void Awake() {
        playerInput = new();
        playerInput.Movement.Enable();
        playerInput.Actions.Enable();

        playerInput.Movement.Dodge.performed += Dodge_Performed;
        playerInput.Actions.MeleeAttack.performed += MeleeAttack_Performed;

        playerInput.Actions.Summon.performed += Summon_Performed;
        playerInput.Actions.SelectSummon.performed += SelectSummon_performed;

        playerInput.Actions.ActivateSkill.started += Skill_Started;
        playerInput.Actions.ActivateSkill.canceled += Skill_Cast;

        Camera.main.TryGetComponent(out cameraBrain);
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

    private void Summon_Performed(InputAction.CallbackContext context) {
        if (context.performed) OnSummonPerformed?.Invoke();
    }

    private void SelectSummon_performed(InputAction.CallbackContext context) {
        if (context.performed) {
            int value = (int) context.ReadValue<float>();
            if (value == -1) OnSummonSelect?.Invoke(SummonType.Battery, 0);
            else if (value > 0) OnSummonSelect?.Invoke(SummonType.Tower, value);
        }
    }

    private void Skill_Started(InputAction.CallbackContext context) {
        if (context.started) OnSkillStarted?.Invoke();
    }

    private void Skill_Cast(InputAction.CallbackContext context) {
        if (context.canceled) OnSkillCast?.Invoke();
    }
}
