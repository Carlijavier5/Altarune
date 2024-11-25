using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerMeleeArea : MonoBehaviour {
    
    private readonly HashSet<BaseObject> collSet = new();

    private Player playerSource;
    private Vector3 SourcePosition => playerSource.transform.position;

    [SerializeField] private int damageAmount;
    [SerializeField] private float manaPerHit;
    [SerializeField] private float staggerDuration,
                                   pushStrength, pushDuration;

    [SerializeField] private Collider areaCollider;
    [SerializeField] private VisualEffect areaParticles;

    private float timer;

    public void DoMelee(Player playerSource, float attackDuration) {
        this.playerSource = playerSource;
        timer = attackDuration;
        areaParticles.Reinit();

        enabled = true;
        areaCollider.enabled = true;
    }

    public void AbortMelee() => Collapse();

    void Update() {
        if (playerSource) {
            timer -= playerSource.DeltaTime;
            if (timer <= 0) Collapse();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)
            && !baseObject.IsFaction(EntityFaction.Friendly)
            && collSet.Add(baseObject) && playerSource) {

            baseObject.TryDamage(damageAmount, ElementType.Siphon);
            playerSource.ManaSource.Fill(manaPerHit);
            baseObject.TryStagger(staggerDuration, true);
            Vector3 direction = baseObject.transform.position - SourcePosition;
            if (baseObject.TryLongPush(direction, pushStrength, pushDuration,
                                       out PushActionCore actionCore)) {
                actionCore.SetEase(EaseCurve.InLogarithmic);
            }
        }
    }

    private void Collapse() {
        enabled = false;
        areaCollider.enabled = false;
        collSet.Clear();
        areaParticles.Stop();
    }
}
