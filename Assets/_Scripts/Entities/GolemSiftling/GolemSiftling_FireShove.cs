using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GolemSiftling
{
    private const string FIRE_CAST_PARAM = "FireCast",
                         FIRE_SHOVE_PARAM = "FireShove",
                         PLAYBACK_MULT_PARAM = "Playback";

    [Header("Fire Shove")]
    [SerializeField] private SiftlingFireTornadoBezierPath fireBezierPath;
    [SerializeField] private LockdownJointTransform fireBezierJoint;

    [System.Serializable]
    private class FireWindUpPhase {
        public float spinDuration;
        public float waitDuration;
        public float targetSpeed;
        public ParticleSystem[] associatedVFXs;

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

    public bool IsTornadoFlipped => TornadoDirectionMultiplier < 0;
    public float TornadoDirectionMultiplier { get; private set; } = 1;

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
            siftling.activeConfig.tornado.ToggleAnimationSync(false);
            siftling.animatorMain.speed = siftling.baseMainAnimatorSpeed;
        }

        private void Tornado_OnRotationSync() {
            siftling.animatorMain.SetFloat(PLAYBACK_MULT_PARAM, -siftling.TornadoDirectionMultiplier);
            siftling.animatorMain.SetTrigger(FIRE_CAST_PARAM);

            siftling.activeConfig.tornado.ToggleAnimationSync(true, siftling.fireCastClip.length);
            DoNextWindUpPhase();
        }

        private void DoNextWindUpPhase() {
            subState = SubState.ChargeUp;
            
            FireWindUpPhase currentPhase = siftling.fireWindUpPhases[windUpPhaseIndex];
            siftling.activeConfig.tornado.AdjustRotationSpeed(currentPhase.targetSpeed, currentPhase.spinDuration);
            nextEventTime = Time.time + currentPhase.spinDuration;
        }
    }

    private class State_FireShove : State<Siftling_Input> {

        private Siftling_Input input;

        public override void Enter(Siftling_Input input) {
            this.input = input;

            input.siftling.animatorMain.SetTrigger(FIRE_SHOVE_PARAM);
            input.siftling.fireBezierJoint.Play();

            float rotationSyncTarget = input.siftling.fireBezierPath.GetPathStartAngle(input.siftling.IsTornadoFlipped);
            input.siftling.activeConfig.tornado.AdjustRotationSyncTarget(rotationSyncTarget);
            input.siftling.activeConfig.tornado.OnRotationSync += Tornado_OnRotationSync;
        }

        public override void Update(Siftling_Input input) { }

        public override void Exit(Siftling_Input input) {
            input.siftling.fireBezierJoint.Stop();
            input.siftling.activeConfig.tornado.Play();

            float duration = input.siftling.fireWindUpPhases.Sum((phase) => phase.spinDuration);
            input.siftling.activeConfig.tornado.ResetRotationSpeed(duration);

            input.siftling.RestartAttackCooldown();
        }

        private void Tornado_OnRotationSync() {
            input.siftling.activeConfig.tornado.Stop();
            input.siftling.fireBezierPath.Play(input.siftling.activeConfig.tornado.RotationSpeed);
            input.siftling.fireBezierPath.OnPathComplete += FireBezierPath_OnPathComplete;
        }

        private void FireBezierPath_OnPathComplete() {
            input.stateMachine.SetState(new State_Idle());
        }
    }
}
