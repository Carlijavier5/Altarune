using UnityEngine;

public abstract class GolemSweepAnimatorLink : MonoBehaviour {

    private const string SWEEP_START_PARAM = "SweepStart",
                         SWEEP_COMMIT_PARAM = "SweepCommit";
    [SerializeField] protected Animator animator;
    [SerializeField] private AnimationClip anticipationClip,
                                           commitClip;

    public float AnticipationLength => anticipationClip.length;
    public float CommitLength => commitClip.length;
    public abstract float DeltaTime { get; }

    public void DoAnticipation(float duration) {
        animator.SetTrigger(SWEEP_START_PARAM);
        AdjustAnticipationAnimatorSpeed(duration);
    }

    public void CommitSweep() {
        animator.SetTrigger(SWEEP_COMMIT_PARAM);
        ResetAnimationSpeed();
    }

    protected abstract void AdjustAnticipationAnimatorSpeed(float logicDuration);
    protected abstract void ResetAnimationSpeed();
}
