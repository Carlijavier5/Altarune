using UnityEngine;

public partial class Player {

    [Header("Normal State")]
    [SerializeField] private LocomotionProperties normalLocomotionProperties;

    private class State_Normal : State<Player_Input> {

        public override void Enter(Player_Input input) {
            Player player = input.player;
            player.driver.ReplaceProperties(player.normalLocomotionProperties);
        }

        public override void Update(Player_Input input) {
            input.player.driver.Move(input.player.InputVector);
            input.player.driver.ResolveRotation();
            input.player.driver.ResolveGravity();
            input.player.animator.SetFloat("MoveSpeed", (input.player.driver.MoveSpeed
                                                       / input.player.normalLocomotionProperties.maxSpeed));
        }

        public override void Exit(Player_Input input) { }
    }
}