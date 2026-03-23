using System.Collections.Generic;
using UnityEngine;

/// Note: May want to switch to a DeltaTime based approach for timers to sync with entity TimeScale;

public partial class GolemSiftling
{
    private readonly int ascendParam = Animator.StringToHash("Ascend"),
                         descendParam = Animator.StringToHash("Descend"),
                         descendWindParam = Animator.StringToHash("DescendWind"),
                         fallParam = Animator.StringToHash("Fall");

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

            gs.oscillator.enabled = false;

            gs.animatorMain.SetTrigger(gs.ascendParam);
            gs.animatorBody.SetTrigger(gs.ascendParam);
            /// Play loop SFX and voice;

            ascendTime = Time.time + gs.ascendClip.length.SafeDivide(gs.animatorBody.speed);

            if (!isFirst) {
                gs.activeConfig.tornado.Toggle(true);
                gs.activeConfig.tornado.OnTornadoSummoned += Tornado_OnTornadoSummoned;
            }
        }

        public override void Update(Siftling_Input _) {
            switch (subState) {
                case SubState.Ascending:
                    if (Time.time >= ascendTime) {
                        if (isFirst) {
                            gs.activeConfig.tornado.Toggle(true);
                            gs.activeConfig.tornado.OnTornadoSummoned += Tornado_OnTornadoSummoned;

                            subState = SubState.Awaiting;
                        } else { /// Triggered either by tornado summoning or as a fallback if the tornado event does not fire;
                            DoDescent();
                            subState = SubState.Awaiting;
                        }
                    }
                    break;
                case SubState.Awaiting:
                    if (canDescend && Time.time >= ascendTime) {
                        int trigger = gs.activeConfig.type == SiftlingType.Wind ? gs.descendWindParam
                                                                                : gs.descendParam;
                        gs.animatorBody.SetTrigger(trigger);
                        gs.animatorMain.SetTrigger(gs.fallParam);

                        if (isFirst) {
                            gs.vfxAscensionLoop.Stop();
                            gs.vfxAscensionExplosion.Play();

                            /// Play ascension end SFX;
                            gs.RemoveMaterial(gs.ascendMaterial);
                            if (gs.activeConfig.crystalMaterial) {
                                gs.crystalRenderer.sharedMaterial = gs.activeConfig.crystalMaterial;
                                gs.UpdateRendererRefs(true);
                            }
                        }

                        gs.animatorBody.speed = gs.baseBodyAnimatorSpeed;
                        subState = SubState.Descending;
                    }
                    break;
                case SubState.Descending:
                    if (Time.time >= descendTime) {

                        if (gs.activeConfig.type == SiftlingType.Wind) {
                            gs.oscillator.UpdateAnchor();
                        }

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

        private void DoDescent() {
            float length = gs.activeConfig.type == SiftlingType.Wind ? gs.descendWindClip.length
                                                                     : gs.descendClip.length;
            /// The animation speed is set to base upon entering the Descend state;
            /// The descend time is either accounted from now or from the moment we are done Ascending;
            descendTime = Mathf.Max(ascendTime, Time.time) + length.SafeDivide(gs.baseMainAnimatorSpeed);
            canDescend = true;
        }

        private void Tornado_OnTornadoSummoned() => DoDescent();
    }
}
