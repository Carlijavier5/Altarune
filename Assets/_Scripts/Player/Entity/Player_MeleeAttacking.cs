using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    [Header("Melee Attack State")]
    [SerializeField] private PlayerMeleeArea meleeArea;
    [SerializeField] private float attackDuration;
    [SerializeField] private LocomotionProperties meleeLocomotionProperties;

    private class State_MeleeAttacking : State<Player_Input> {

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

            input.player.meleeArea.DoMelee(input.player, input.player.attackDuration);
        }

        public override void Update(Player_Input input) {
            float amountDelta = input.player.DeltaTime / input.player.attackDuration;
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
                                           / input.player.normalLocomotionProperties.maxSpeed));
        }

        public override void Exit(Player_Input input) {
            input.player.meleeArea.AbortMelee();
            input.player.SetTopLayerWeight(layer, 0, 10);
        }
    }
}