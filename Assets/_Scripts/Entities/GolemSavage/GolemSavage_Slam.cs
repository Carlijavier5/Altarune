using System.Collections;
using UnityEngine;

public partial class GolemSavage {

    private const string SLAM_TRIGGER = "Slam";

    [Header("Slam")]
    [SerializeField] private PawSlam slamWave;
    [SerializeField] private AnimationClip slamClip;

    private class State_Slam : State<Savage_Input> {

        private readonly SavagePhase phase;
        private float timer;

        public State_Slam(SavagePhase phase) {
            this.phase = phase;
        }

        public override void Enter(Savage_Input input) {
            Debug.Log("Did slam");
            GolemSavage gs = input.savage;
            gs.navMeshAgent.ResetPath();
            gs.navMeshAgent.enabled = false;
            gs.animator.SetTrigger(SLAM_TRIGGER);
            timer = gs.slamClip.length;
        }

        public override void Update(Savage_Input input) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                float attackTime = input.savage.activeConfig.RandomAttackTime;
                input.microMachine.SetState(new State_Idle(phase, attackTime));
            }
        }

        public override void Exit(Savage_Input input) {
            input.savage.navMeshAgent.enabled = true;
        }

        /*
        public void LookTowardsPlayer() {
            // Determines how the enemy should rotate towards the player (ignoring Y)
            float rotateX = player.position.x - gs.transform.position.x;
            float rotateZ = player.position.z - gs.transform.position.z;
            Vector3 directionToPlayer = new Vector3(rotateX, 0f, rotateZ).normalized;
            Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);

            // Keeps the enemy's Y direction the same
            float currentYRotation = gs.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, rotateToPlayer.eulerAngles.y, 0f);

            // Rotates the enemy towards the player (uses slerp)
            gs.transform.rotation = Quaternion.Slerp(gs.transform.rotation, rotateToPlayer, 2f * gs.DeltaTime);
        }
        */
    }
}