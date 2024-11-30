using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class Player : Entity {

    private readonly StateMachine<Player_Input> stateMachine = new();

    [SerializeField] private PlayerController inputSource;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;

    [SerializeField] private ManaSource manaSource;
    [SerializeField] private float manaGain, maxMana;
    [SerializeField] private ManaUIManager manaUIManager;

    public ManaSource ManaSource => manaSource;
    public PlayerController InputSource => inputSource;

    private Vector3 InputVector => inputSource.InputVector;

    private readonly Dictionary<int, Coroutine> layerWeightCoroutineMap = new();
    private PlayerLocomotionDriver driver;

    private void Awake() {
        inputSource.OnPlayerInit += PlayerController_OnPlayerInit;
        ManaSource.Init(maxMana);
    }

    private void PlayerController_OnPlayerInit() {
        driver = new(this);
        Player_Input input = new(stateMachine, this);
        stateMachine.Init(input, new State_Normal());

        inputSource.OnDodgePerformed += InputSource_OnDodgePerformed;
        inputSource.OnMeleePerformed += InputSource_OnMeleePerformed;
    }

    private void InputSource_OnMeleePerformed() {
        if (stateMachine.State is State_Normal) {
            stateMachine.SetState(new State_MeleeAttacking());
        }
    }

    private void InputSource_OnDodgePerformed() {
        if (stateMachine.State is State_Normal
            || stateMachine.State is State_MeleeAttacking
            || stateMachine.State is State_Summoning) {
            stateMachine.SetState(new State_Rolling());
        }
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
        ManaSource.Fill(Time.deltaTime * manaGain);

        if (Input.GetKeyDown(KeyCode.J) && stateMachine.State is State_Normal) {
            stateMachine.SetState(new State_Summoning());
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            if (stateMachine.State is State_Normal) stateMachine.SetState(new State_Burdened());
            else if (stateMachine.State is State_Burdened) stateMachine.SetState(new State_Normal());
        }
    }

    void FixedUpdate() {
        stateMachine.FixedUpdate();
    }

    private void SetTopLayerWeight(int layer, float target, float speed) {
        if (layerWeightCoroutineMap.ContainsKey(layer)) StopCoroutine(layerWeightCoroutineMap[layer]);
        layerWeightCoroutineMap[layer] = StartCoroutine(IRaiseLayerWeight(layer, target, speed));
    }

    private IEnumerator IRaiseLayerWeight(int layer, float target, float speed) {
        float weight = animator.GetLayerWeight(layer);
        while (weight != target) {
            weight = Mathf.MoveTowards(weight, target, DeltaTime * speed);
            animator.SetLayerWeight(layer, weight);
            yield return null;
        }
    }
}