using System.Collections;
using UnityEngine;

public class GolemSweep : MonoBehaviour {

    public event System.Action OnSweepEnd;
    public event System.Action OnCooldownEnd;

    [SerializeField] private GolemSweepAnimatorLink animatorLink;
    [SerializeField] private GolemSweepHitbox hitbox;
    [SerializeField] private CastRangeEnforcer castRangeEnforcer;
    [SerializeField] private int damageAmount;
    [SerializeField] private float sweepDuration = 0.3f,
                                   sweepCooldown, interruptStunDuration;

    public bool IsAvailable => cooldownTimer <= 0 && !hitbox.Active;
    private float cooldownTimer;

    void Awake() {
        transform.SetParent(null);
        hitbox.OnAnticipationEnd += Hitbox_OnAnctipationEnd;
        castRangeEnforcer.OnContactCancel += CastRangeEnforcer_OnContactCancel;
    }

    private void CastRangeEnforcer_OnContactCancel(Entity entity) {
        CancelSweep();
        entity.ApplyEffects(new[] { 
            new StunStatusEffect(interruptStunDuration)
        });
    }

    public void DoSweep(Entity caster, Quaternion lookRotation) {
        StopAllCoroutines();
        StartCoroutine(IDoCooldown(caster));
        ArrangeHitbox(caster, lookRotation);
        castRangeEnforcer.Toggle(true);
        hitbox.DoAnticipation(caster, sweepDuration);
        animatorLink.DoAnticipation(sweepDuration);
    }

    public void CancelSweep() {
        hitbox.CancelSweep();
        OnSweepEnd?.Invoke();
    }

    private void ArrangeHitbox(Entity caster, Quaternion lookRotation) {
        transform.SetPositionAndRotation(caster.transform.position, lookRotation);
    }

    private void Hitbox_OnAnctipationEnd() {
        animatorLink.CommitSweep();
        castRangeEnforcer.Toggle(false);
        hitbox.DoDamage(damageAmount);
        StartCoroutine(IAfterSweepWait());
    }

    private IEnumerator IAfterSweepWait() {
        float timer = animatorLink.CommitLength;
        while (timer > 0) {
            timer -= animatorLink.DeltaTime;
            yield return null;
        }
        OnSweepEnd?.Invoke();
    }

    private IEnumerator IDoCooldown(Entity caster) {
        cooldownTimer = sweepCooldown;
        while (cooldownTimer > 0) {
            cooldownTimer -= caster.DeltaTime;
            yield return null;
        } OnCooldownEnd?.Invoke();
    }
}