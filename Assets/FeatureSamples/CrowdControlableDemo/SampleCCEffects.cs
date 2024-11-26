using UnityEngine;

namespace FeatureSamples {

    /// The following effects showcase how to modify the CCEffects
    /// parameter in the StatusEffects class to produce stuns, roots, and slows!

    public class SampleStunEffect : EntityEffect {

        private float effectDuration;

        public SampleStunEffect(float duration) {
            effectDuration = duration;
        }

        public override void Apply(Entity entity, bool isNew) {
            CCEffects = new() { stun = new() { duration = effectDuration } };
            ///
            /// Instantiates a 'stun' node and sets its duration to match
            /// the effect duration. This duration can be shorter than that of
            /// the effect.
            /// 
            /// Note that, if the effect is removed, the stun will be removed;
        }

        public override bool Update(Entity entity) {
            effectDuration -= Time.deltaTime;
            return effectDuration <= 0;
        }

        public override void Terminate(Entity entity) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SampleRootEffect : EntityEffect {

        private float effectDuration;

        public SampleRootEffect(float duration) {
            effectDuration = duration;
        }

        public override void Apply(Entity entity, bool isNew) {
            CCEffects = new() { root = new() { duration = effectDuration } };
            ///
            /// Instantiates a 'root' node and sets its duration to match
            /// the effect duration. This duration can be shorter than that of
            /// the effect.
            /// 
            /// Note that, if the effect is removed, the root will be removed;
        }

        public override bool Update(Entity entity) {
            effectDuration -= Time.deltaTime;
            return effectDuration <= 0;
        }

        public override void Terminate(Entity entity) { }
    }

    public class SampleSlowEffect : EntityEffect {

        private readonly float effectDuration;
        private float timer;

        private readonly float slowMult;

        public SampleSlowEffect(float duration, float slowMult) {
            effectDuration = duration;
            timer = duration;
        }

        public override void Apply(Entity entity, bool isNew) {
            CCEffects = new() {
                slow = new() { duration = effectDuration,
                               Multiplier = slowMult }
            };
            ///
            /// Instantiates a 'slow' node and sets its duration to match
            /// the effect duration (which may be shorter that the effect duration).
            /// Additionally, it sets the Multiplier that will be applied.
            /// 
            /// The Multiplier can be any number between 0.1 and 10.
            /// Numbers between 0.1 and 1 will slow down the entity.
            /// Numbers between 1 and 10 will speed up the entity.
            /// Any number out range can be safely passed, but it will clamped.
            /// 
            /// Note that, if the effect is removed, the slow will be removed;
        }

        public override bool Update(Entity entity) {

            float lerpVal = Mathf.Clamp01(timer / effectDuration);
            CCEffects.slow.Multiplier = Mathf.Lerp(1, slowMult, lerpVal);
            ///
            /// Updates the Multiplier in the Slow node every frame.
            /// This node is read dynamically by the CC Module,
            /// so any changes to it will be reflected there in real time.

            timer -= Time.deltaTime;
            return timer <= 0;
        }

        public override void Terminate(Entity entity) { }
    }
    
    /// Important Notes on CC Features for Status Effects
    /// 
    /// - The duration of CC effects it's computed by the local Crowd Control module.
    /// - Applied effects are subject to diminishing returns and resistances, so the
    ///   exact duration of each effect may vary. If you would like your effect duration
    ///   to match your CC duration, update your effect duration on Start.
    /// - Only the CCEffects assigned on Apply will be subject to diminishing returns.
    /// - You can have multiple CC nodes active in one effect.
}
