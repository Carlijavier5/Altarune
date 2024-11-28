using UnityEngine;

public partial class SovereignGolem {

    [Header("Intro")]
    [SerializeField] private AnimationClip introClip;
    [SerializeField] private SFXOneShot sfxActivation;
    [SerializeField] private MusicTrigger musicTrigger;
    public class MacroState_Inert : MacroState {

        public override SovereignPhase Phase => SovereignPhase.None;
        private float timer;

        public override void Enter(Sovereign_Input input) {
            SovereignGolem sg = input.sovereign;
            sg.sfxActivation.Play();
            sg.musicTrigger.Play(sg.sfxActivation.Clip.length);
        }

        public override void Update(Sovereign_Input input) {
            timer += Time.deltaTime;
            if (timer > input.sovereign.introClip.length) {
                input.sovereign.DoMacroTransition();
            }
        }
    }
}