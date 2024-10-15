using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class Player : Entity {

    private readonly StateMachine<Player_Input> stateMachine = new();

    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerController inputSource;
    [SerializeField] private Animator animator;

    private PlayerLocomotionDriver driver;

    private Vector3 InputVector => inputSource.InputVector;

    private void Awake() {
        inputSource.OnPlayerInit += PlayerController_OnPlayerInit;
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
            stateMachine.SetState(new State_MeleeAttack());
        }
    }

    private void InputSource_OnDodgePerformed() {
        if (stateMachine.State is State_Normal
            || stateMachine.State is State_MeleeAttack) {
            stateMachine.SetState(new State_Roll());
        }
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
    }
}

public partial class Player {

    [SerializeField] private LocomotionProperties meleeLocomotionProperties;

    private readonly Dictionary<int, Coroutine> layerWeightCoroutineMap = new();

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

    private class State_MeleeAttack : State<Player_Input> {

        private readonly float attackDuration = 0.31f;
        private int layer;

        private Vector3 dir;
        private float amount;

        public override void Enter(Player_Input input) {
            input.player.driver.ReplaceProperties(input.player.meleeLocomotionProperties);
            Vector2 inputVector = input.player.InputVector;
            dir = inputVector.magnitude > 0 ? input.player.inputSource.InputVector
                                            : input.player.driver.MoveDir;
            layer = input.player.driver.MoveSpeed > 0 ? 1 : 2;
            input.player.animator.SetTrigger("MeleeAttack");
            input.player.SetTopLayerWeight(layer, 1, 20);
        }

        public override void Update(Player_Input input) {
            float amountDelta = Time.deltaTime / attackDuration;
            bool performed = (amount = Mathf.MoveTowards(amount, 1, amountDelta)) == 1;
            if (performed) {
                /// Passing a 0-magnitude vector stops the motion;
                input.player.driver.Move(Vector2.zero);
                if (input.player.driver.MoveSpeed <= 0) {
                    input.stateMachine.SetState(new State_Normal());
                }
            } else {
                input.player.driver.Move(dir);
            }
            input.player.driver.ResolveRotation();
            input.player.driver.ResolveGravity();
            input.player.animator.SetFloat("MoveSpeed", (input.player.driver.MoveSpeed
                                           / input.player.normalMotionProperties.maxSpeed));
        }

        public override void Exit(Player_Input input) {
            input.player.SetTopLayerWeight(layer, 0, 10);
        }
    }
}

public partial class Player {
    private class State_CarryItem : State<Player_Input> {

        public override void Enter(Player_Input input) {
            throw new System.NotImplementedException();
        }

        public override void Update(Player_Input input) {
            throw new System.NotImplementedException();
        }

        public override void Exit(Player_Input input) {
            throw new System.NotImplementedException();
        }
    }
}

public partial class Player {
    private class State_Armed : State<Player_Input> {

        public override void Enter(Player_Input input) {
            throw new System.NotImplementedException();
        }

        public override void Update(Player_Input input) {
            throw new System.NotImplementedException();
        }

        public override void Exit(Player_Input input) {
            throw new System.NotImplementedException();
        }
    }
}