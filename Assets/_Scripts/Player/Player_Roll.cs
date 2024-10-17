using UnityEngine;

public partial class Player {

    [SerializeField] private LocomotionProperties dodgeLocomotionProperties;
    private readonly float dodgeDuration = 0.2f;

    private class State_Roll : State<Player_Input> {

        private float amount;
        private Vector3 dir;

        public override void Enter(Player_Input input) {
            input.player.driver.ReplaceProperties(input.player.dodgeLocomotionProperties);
            Vector2 inputVector = input.player.InputVector;
            dir = inputVector.magnitude > 0 ? input.player.inputSource.InputVector
                                            : input.player.driver.MoveDir;
            input.player.animator.Play("Dodge");
        }

        public override void Update(Player_Input input) {
            float amountDelta = Time.deltaTime / input.player.dodgeDuration;
            bool performed = (amount = Mathf.MoveTowards(amount, 1, amountDelta)) == 1;
            if (performed) {
                /// Passing a 0-magnitude vector stops the motion;
                input.player.driver.Move(Vector2.zero);
                if (input.player.driver.MoveSpeed <= input.player.normalMotionProperties.maxSpeed / 2) {
                    input.stateMachine.SetState(new State_Normal());
                }
            } else {
                input.player.driver.Move(dir);
            }
            input.player.driver.ResolveRotation();
            input.player.driver.ResolveGravity();
        }

        public override void Exit(Player_Input input) { }
    }
}