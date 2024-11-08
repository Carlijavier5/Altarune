using UnityEngine;

public partial class Player {

    [Header("Burdened State")]
    [SerializeField] private LocomotionProperties burdenedLocomotionProperties;

    private class State_Burdened : State<Player_Input> {

        public override void Enter(Player_Input input) {
            input.player.driver.ReplaceProperties(input.player.burdenedLocomotionProperties);
        }

        public override void Update(Player_Input input) {
            input.player.driver.ResolveRotation();
            input.player.animator.SetFloat("MoveSpeed", (input.player.driver.MoveSpeed
                                                       / input.player.normalLocomotionProperties.maxSpeed));
        }

        public override void FixedUpdate(Player_Input input) {
            input.player.driver.Move(input.player.InputVector);
            input.player.driver.ResolveGravity();
        }

        public override void Exit(Player_Input input) { }
    }
}