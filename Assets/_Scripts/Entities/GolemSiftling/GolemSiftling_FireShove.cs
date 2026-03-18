using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GolemSiftling {
    private const string FIRE_CAST_PARAM = "FireCast",
                         FIRE_SHOVE_PARAM = "FireShove",
                         DIRECTION_PARAM = "Direction";

    [Header("Fire Shove")]
    [SerializeField] private SiftlingFireTornadoBezierPath fireBezierPath;
    [SerializeField] private LockdownJointTransform fireBezierJoint;

    [SerializeField] private ParticleSystem exclamationVFX;
    [SerializeField] private TwoColoredGraphicFader[] fireAnticipationGraphics;

    [System.Serializable]
    private class FireWindUpPhase {
        public float spinDuration;
        public float waitDuration;
        public float targetSpeed;

        public ParticleSystem[] associatedVFXs;
        public bool preserveIndicators;

        public void PlayVFXs() {
            foreach (ParticleSystem vfx in associatedVFXs) {
                vfx.Play();
            }
        }

        public void StopVFXs() {
            foreach (ParticleSystem vfx in associatedVFXs) {
                vfx.Stop();
            }
        }
    }

    [SerializeField] private FireWindUpPhase[] fireWindUpPhases;
    [SerializeField] private AnimationClip fireCastClip;
    [SerializeField] private SiftlingFireCircleController fireCircleController;
    [SerializeField] private ParticleSystem fireDispersionVFXRoot;
    [SerializeField] private ParticleSystemCollection fireStreamVFXCollection;

    public bool IsTornadoFlipped => TornadoDirectionMultiplier < 0;
    public float TornadoDirectionMultiplier { get; private set; } = 1;

    private void StopFireWindUpVFXs() {
        fireCircleController.Stop();

        foreach (FireWindUpPhase phase in fireWindUpPhases) {
            phase.StopVFXs();
        }
    }

    private class State_FireWindUp : State<Siftling_Input> {

        private GolemSiftling siftling;

        private enum SubState { ChargeUp, Await }
        private SubState subState;

        private float nextEventTime;
        private int windUpPhaseIndex;

        public override void Enter(Siftling_Input input) {
            siftling = input.siftling;

            siftling.activeConfig.tornado.AdjustRotationSyncTarget(0);
            siftling.activeConfig.tornado.OnRotationSync += Tornado_OnRotationSync;
            nextEventTime = Mathf.Infinity;
        }

        public override void Update(Siftling_Input input) {
            if (Time.time >= nextEventTime) {
                input.siftling.fireCircleController.Lock();
                switch(subState) {
                    case SubState.ChargeUp:
                        nextEventTime = Time.time + siftling.fireWindUpPhases[windUpPhaseIndex].waitDuration;
                        subState = SubState.Await;
                        break;
                    case SubState.Await:
                        windUpPhaseIndex++;
                        if (windUpPhaseIndex < siftling.fireWindUpPhases.Length) {
                            DoNextWindUpPhase();
                        } else {
                            input.stateMachine.SetState(new State_FireShove());
                        }
                        break;
                }
            }
        }

        public override void Exit(Siftling_Input input) {
            siftling.activeConfig.tornado.OnRotationSync -= Tornado_OnRotationSync;

            siftling.TogglePhaseAttackIndicators(false);

            if (input.stateMachine.NextState is not State_FireShove) {
                siftling.activeConfig.tornado.ToggleAnimationSync(false);
                siftling.ResetMainAnimatorSpeed();

                siftling.StopFireWindUpVFXs();

                if (windUpPhaseIndex > 0) {
                    siftling.fireDispersionVFXRoot.Play();
                }
            }
        }

        private void Tornado_OnRotationSync() {
            siftling.activeConfig.tornado.OnRotationSync -= Tornado_OnRotationSync;

            siftling.animatorMain.SetFloat(DIRECTION_PARAM, -siftling.TornadoDirectionMultiplier);
            siftling.animatorMain.SetTrigger(FIRE_CAST_PARAM);

            siftling.activeConfig.tornado.ToggleAnimationSync(true, siftling.fireCastClip.length);

            DoNextWindUpPhase();
            siftling.fireCircleController.Play(siftling.fireWindUpPhases[windUpPhaseIndex].spinDuration);
        }

        private void DoNextWindUpPhase() {
            subState = SubState.ChargeUp;

            FireWindUpPhase currentPhase = siftling.fireWindUpPhases[windUpPhaseIndex];
            siftling.activeConfig.tornado.AdjustRotationSpeed(currentPhase.targetSpeed, currentPhase.spinDuration);

            if (!currentPhase.preserveIndicators) siftling.TogglePhaseAttackIndicators(false);

            nextEventTime = Time.time + currentPhase.spinDuration;

            currentPhase.PlayVFXs();
        }
    }

    private class State_FireShove : State<Siftling_Input> {

        private Siftling_Input input;

        public override void Enter(Siftling_Input input) {
            this.input = input;

            input.siftling.fireBezierJoint.Play();

            float rotationSyncTarget = input.siftling.fireBezierPath.GetPathStartAngle(!input.siftling.IsTornadoFlipped);
            input.siftling.activeConfig.tornado.AdjustRotationSyncTarget(rotationSyncTarget);

            input.siftling.activeConfig.tornado.OnRotationSync += Tornado_OnRotationSync_Animation;
        }

        public override void Update(Siftling_Input input) { }

        public override void Exit(Siftling_Input input) {
            input.siftling.activeConfig.tornado.OnRotationSync -= Tornado_OnRotationSync_Animation;
            input.siftling.activeConfig.tornado.OnRotationSync -= Tornado_OnRotationSync_PlayPath;
            input.siftling.fireBezierPath.OnPathEndpoint -= FireBezierPath_OnPathEndpoint;
            input.siftling.fireBezierPath.OnPathComplete -= FireBezierPath_OnPathComplete;

            input.siftling.fireBezierJoint.Stop();
            input.siftling.activeConfig.tornado.Play();

            float duration = input.siftling.fireWindUpPhases.Sum((phase) => phase.spinDuration);
            input.siftling.activeConfig.tornado.ResetRotationSpeed(duration);

            input.siftling.RestartAttackCooldown();

            input.siftling.activeConfig.tornado.ToggleAnimationSync(false);
            input.siftling.ResetMainAnimatorSpeed();

            input.siftling.StopFireWindUpVFXs();
            input.siftling.fireStreamVFXCollection.Stop();
        }

        private void Tornado_OnRotationSync_Animation() {
            input.siftling.activeConfig.tornado.OnRotationSync -= Tornado_OnRotationSync_Animation;

            input.siftling.StopFireWindUpVFXs();
            input.siftling.fireDispersionVFXRoot.Play();

            input.siftling.activeConfig.tornado.ToggleAnimationSync(false);
            input.siftling.ResetMainAnimatorSpeed();
            input.siftling.animatorMain.SetTrigger(FIRE_SHOVE_PARAM);

            input.siftling.fireStreamVFXCollection.Play();

            float rotationSyncTarget = input.siftling.fireBezierPath.GetPathStartAngle(input.siftling.IsTornadoFlipped);
            input.siftling.activeConfig.tornado.AdjustRotationSyncTarget(rotationSyncTarget);

            input.siftling.activeConfig.tornado.OnRotationSync += Tornado_OnRotationSync_PlayPath;
        }

        private void Tornado_OnRotationSync_PlayPath() {
            input.siftling.activeConfig.tornado.OnRotationSync -= Tornado_OnRotationSync_PlayPath;

            input.siftling.activeConfig.tornado.Stop();
            input.siftling.fireBezierPath.Play(input.siftling.activeConfig.tornado.RotationSpeed);

            input.siftling.fireBezierPath.OnPathEndpoint += FireBezierPath_OnPathEndpoint;
            input.siftling.fireBezierPath.OnPathComplete += FireBezierPath_OnPathComplete;
        }

        private void FireBezierPath_OnPathEndpoint() {
            input.siftling.fireBezierPath.OnPathEndpoint -= FireBezierPath_OnPathEndpoint;
            input.siftling.fireStreamVFXCollection.Stop();
        }

        private void FireBezierPath_OnPathComplete() {
            input.stateMachine.SetState(new State_Idle());
        }
    }
}
