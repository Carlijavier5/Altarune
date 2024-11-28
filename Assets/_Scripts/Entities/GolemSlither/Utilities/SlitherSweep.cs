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
        ArrangeHitbox(caster, lookRotation);
        castRangeEnforcer.Toggle(true);
        hitbox.DoAnticipation(caster, sweepDuration);
    }

    private void ArrangeHitbox(Entity caster, Quaternion lookRotation) {
        transform.position = caster.transform.position;
        transform.rotation = lookRotation;
    }

    private void Hitbox_OnAnctipationEnd() {
        castRangeEnforcer.Toggle(false);
        hitbox.DoDamage(damageAmount);
        StopAllCoroutines();
        StartCoroutine(IDoCooldown());
    }

    private IEnumerator IDoCooldown() {
        yield return new WaitForSeconds(sweepCooldown);
        OnCooldownEnd?.Invoke();
    }
}