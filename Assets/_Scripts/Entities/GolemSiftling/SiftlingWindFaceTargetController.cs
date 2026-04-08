using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiftlingWindFaceTargetController : MonoBehaviour
{
    private readonly int stepParam = Animator.StringToHash("RaiseLeg");

    [SerializeField] private GolemSiftling siftling;
    [SerializeField] private Transform siftlingBody;
    [SerializeField] private Animator animatorMain, animatorBody;
    [Tooltip("At which percent of the animation should it be locked;")]
    [SerializeField] private float raisedLegPercent;
    [Tooltip("At which offset from the animation should a slowdown be applied to the animation;")]
    [SerializeField] private float raiseLegSlowdownBuffer;
    [Tooltip("How long does the tornado spin descent last;")]
    [SerializeField] private float descentDuration;
    [Tooltip("At which percent of the duration is the attack direction locked and indicators shown;")]
    [SerializeField] private float descentDirectionLockPercent;
    [Tooltip("How many complete revolutions should the siftling perform as it descends;")]
    [SerializeField] private float revolutionsPerDescent;

    private float turnDirectionMultiplier;

    public void Play(Entity aggroTarget) {
        
    }

    private void FlipTurnDirection() {
        turnDirectionMultiplier = Random.value > 0.5f ? 1 : -1;
    }
}
