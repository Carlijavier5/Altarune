using UnityEngine;

public partial class Player {

    [Header("Summon State")]
    [SerializeField] private float summonDuration;

    private class State_Summoning : State<Player_Input> {

        private int layer;

        private float amount;

        public override void Enter(Player_Input input) {
            input.player.driver.ReplaceProperties(input.player.normalLocomotionProperties);
            layer = input.player.driver.MoveSpeed > 0 ? 1 : 2;
            input.player.animator.SetTrigger("CastSpell");
            input.player.SetTopLayerWeight(layer, 1, 20);
        }

        public override void Update(Player_Input input) {
            input.player.driver.ResolveRotation();
            input.player.animator.SetFloat("MoveSpeed", (input.player.driver.MoveSpeed
                                           / input.player.normalLocomotionProperties.maxSpeed));
        }

        public override void FixedUpdate(Player_Input input) {
            input.player.driver.Move(input.player.InputVector);
            input.player.driver.ResolveGravity();

            float amountDelta = input.player.FixedDeltaTime / input.player.summonDuration;
            bool performed = (amount = Mathf.MoveTowards(amount, 1, amountDelta)) == 1;
            if (performed) {
                input.stateMachine.SetState(new State_Normal());
            }
        }

        public override void Exit(Player_Input input) {
            input.player.SetTopLayerWeight(layer, 0, 10);
        }
    }
}