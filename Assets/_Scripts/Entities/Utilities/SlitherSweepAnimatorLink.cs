using UnityEngine;

public class SlitherSweepAnimatorLink : GolemSweepAnimatorLink {

    [SerializeField] private GolemSlither golem;

    public override float DeltaTime => golem.DeltaTime;

    /// Set animation speed to match logic length;
    protected override void AdjustAnticipationAnimatorSpeed(float logicDuration) {
        golem.BaseAnimatorSpeed = AnticipationLength / logicDuration;
    }

    /// Reset speed back to TimeScale;
    protected override void ResetAnimationSpeed() {
        golem.BaseAnimatorSpeed = 1;
    }
}