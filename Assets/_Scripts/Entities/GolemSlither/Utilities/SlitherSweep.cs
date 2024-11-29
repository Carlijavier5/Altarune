using System.Collections;
using UnityEngine;

public class SlitherSweep : MonoBehaviour {

    public event System.Action OnCooldownEnd;

    [SerializeField] private Animator animator;
    [SerializeField] private SlitherSweepHitbox hitbox;
    [SerializeField] private CastRangeEnforcer castRangeEnforcer;
    [SerializeField] private int damageAmount;
    [SerializeField] private float sweepDuration = 0.3f,
                                   sweepCooldown, interruptStunDuration;
    public bool Available => timer <= 0 && !hitbox.Active;
    private float timer;

    void Awake() {
        hitbox.OnAnticipationEnd += Hitbox_OnAnctipationEnd;
        castRangeEnforcer.OnContactCancel += CastRangeEnforcer_OnContactCancel;
    }

    private void CastRangeEnforcer_OnContactCancel(Entity entity) {
        hitbox.CancelSweep();
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
    }

    public void CancelSweep() => hitbox.CancelSweep();

    private void ArrangeHitbox(Entity caster, Quaternion lookRotation) {
        transform.SetPositionAndRotation(caster.transform.position, lookRotation);
    }

    private void Hitbox_OnAnctipationEnd() {
        castRangeEnforcer.Toggle(false);
        hitbox.DoDamage(damageAmount);
    }

    private IEnumerator IDoCooldown(Entity caster) {
        timer = sweepCooldown;
        while (timer > 0) {
            timer -= caster.DeltaTime;
            yield return null;
        } OnCooldownEnd?.Invoke();
    }
}