using System.Collections.Generic;
using UnityEngine;

/// Note: May want to switch to a DeltaTime based approach for timers to sync with entity TimeScale;

public partial class GolemSiftling
{
    private const string ASCEND_PARAM = "Ascend",
                         DESCEND_PARAM = "Descend",
                         DESCEND_WIND_PARAM = "DescendWind",
                         FALL_PARAM = "Fall";

    [Header("Ascension State")]
    [SerializeField] private Material ascendMaterial;
    [SerializeField] private Renderer crystalRenderer;
    [SerializeField] private AnimationClip ascendClip, descendClip, descendWindClip;
    [SerializeField] private ParticleSystemCollection vfxAscensionLoop;
    [SerializeField] private ParticleSystem vfxAscensionExplosion;
    [SerializeField] private Oscillator oscillator;
    [SerializeField] private float secondaryAscensionSpeed;

    private class State_Ascend : State<Siftling_Input> {

        private enum SubState { Ascending, Awaiting, Descending }
        private readonly bool isFirst;

        private GolemSiftling gs;
        private SubState subState = SubState.Ascending;
        private float ascendTime, descendTime;
        private bool canDescend;

        public State_Ascend(bool isFirst) {
            this.isFirst = isFirst;
        }

        public override void Enter(Siftling_Input input) {
            gs = input.siftling;

            if (isFirst) {
                gs.TryToggleIFrame(true);
                gs.RestartAttackCooldown();

                gs.ApplyMaterial(gs.ascendMaterial);
                gs.vfxAscensionLoop.Play();
            }

            gs.animatorBody.enabled = true;
            gs.animatorBody.speed = isFirst ? gs.baseBodyAnimatorSpeed : gs.secondaryAscensionSpeed;

            gs.animatorMain.SetTrigger(ASCEND_PARAM);
            gs.animatorBody.SetTrigger(ASCEND_PARAM);
            /// Play loop SFX and voice;

            ascendTime = Time.time + gs.ascendClip.length.SafeDivide(gs.animatorBody.speed);
        }

        public override void Update(Siftling_Input _) {
            switch (subState) {
                case SubState.Ascending:
                    if (!isFirst || Time.time >= ascendTime) {
                        gs.activeConfig.tornado.Toggle(true);
                        gs.activeConfig.tornado.OnTornadoSummoned += Tornado_OnTornadoSummoned;

                        subState = SubState.Awaiting;
                    }
                    break;
                case SubState.Awaiting:
                    if (canDescend && Time.time >= ascendTime) {
                        gs.animatorBody.enabled = true;
                        gs.oscillator.enabled = false;

                        string trigger = gs.activeConfig.type == SiftlingType.Wind ? DESCEND_WIND_PARAM
                                                                                   : DESCEND_PARAM;
                        gs.animatorBody.SetTrigger(trigger);
                        gs.animatorMain.SetTrigger(FALL_PARAM);

                        if (isFirst) {
                            gs.vfxAscensionLoop.Stop();
                            gs.vfxAscensionExplosion.Play();
                        }
                        /// Play ascension end SFX;

                        gs.animatorBody.speed = gs.baseBodyAnimatorSpeed;
                        gs.RemoveMaterial(gs.ascendMaterial);
                        if (gs.activeConfig.crystalMaterial) {
                            gs.crystalRenderer.sharedMaterial = gs.activeConfig.crystalMaterial;
                            gs.UpdateRendererRefs(true);
                        }

                        subState = SubState.Descending;
                    } else if (Time.time >= ascendTime) {
                        gs.animatorBody.enabled = false;
                        gs.oscillator.enabled = true;
                    }
                    break;
                case SubState.Descending:
                    if (Time.time >= descendTime) {
                        gs.stateMachine.SetState(new State_Idle());
                    }
                    break;
            }
        }

        public override void Exit(Siftling_Input input) {
            if (isFirst) {
                gs.TryToggleIFrame(false);
                gs.vfxAscensionLoop.Stop();
            }
            gs.oscillator.enabled = false;
        }

        private void Tornado_OnTornadoSummoned() {
            float length = gs.activeConfig.type == SiftlingType.Wind ? gs.descendWindClip.length
                                                                     : gs.descendClip.length;
            /// The animation speed is set to base upon entering the Descend state;
            /// The descend time is either accounted from now or from the moment we are done Ascending;
            descendTime = Mathf.Max(ascendTime, Time.time) + length.SafeDivide(gs.baseMainAnimatorSpeed);
            canDescend = true;
        }
    }
}