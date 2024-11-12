using UnityEngine;

public partial class Player {

    [Header("Dodge Roll State")]
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private LocomotionProperties dodgeLocomotionProperties;

    private class State_Rolling : State<Player_Input> {

        private float amount;
        private Vector3 dir;

        public override void Enter(Player_Input input) {
            input.player.TryToggleIFrame(true);
            input.player.driver.ReplaceProperties(input.player.dodgeLocomotionProperties);
            Vector2 inputVector = input.player.InputVector;
            dir = inputVector.magnitude > 0 ? input.player.inputSource.InputVector
                                            : input.player.driver.MoveDir;
            input.player.animator.Play("Dodge");
        }

        public override void Update(Player_Input input) {
            input.player.driver.ResolveRotation();
        }

        public override void FixedUpdate(Player_Input input) {
            float amountDelta = input.player.FixedDeltaTime / input.player.dodgeDuration;
            bool performed = (amount = Mathf.MoveTowards(amount, 1, amountDelta)) == 1;
            if (performed) {
                /// Passing a 0-magnitude vector stops the motion;
                input.player.driver.Move(Vector2.zero);
                if (input.player.driver.MoveSpeed <= input.player.normalLocomotionProperties.maxSpeed *  0.75f) {
                    input.stateMachine.SetState(new State_Normal());
                }
            } else {
                input.player.driver.Move(dir);
            }
        }

        public override void Exit(Player_Input input) {
            input.player.TryToggleIFrame(false);
        }
    }
}