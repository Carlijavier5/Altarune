using UnityEngine;

public partial class GolemSavage {

    private const string METEOR_HURL_PARAM = "MeteorHurl";

    [Header("Meteor Hurl")]
    [SerializeField] private MeteorSpawner meteorSpawner;
    [SerializeField] private AnimationClip meteorHurlClip;

    private class State_MeteorHurl : State<Savage_Input> {

        private readonly SavagePhase phase;
        private float timer;

        public State_MeteorHurl(SavagePhase phase) {
            this.phase = phase;
        }

        public override void Enter(Savage_Input input) {
            input.savage.navMeshAgent.ResetPath();
            input.savage.navMeshAgent.enabled = false;
            SavagePhaseConfiguration config = input.savage.activeConfig;
            input.savage.meteorSpawner.DoMeteorHurl(config.meteorAmount,
                                                    config.meteorRiseInterval,
                                                    config.meteorRiseDuration,
                                                    config.meteorFallDuration);
            input.savage.animator.SetTrigger(METEOR_HURL_PARAM);
            timer = input.savage.meteorHurlClip.length;
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