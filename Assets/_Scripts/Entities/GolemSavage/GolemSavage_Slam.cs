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
    }
}