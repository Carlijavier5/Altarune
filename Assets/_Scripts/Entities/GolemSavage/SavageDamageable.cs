using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GolemSavage))]
public class SavageDamageable : Damageable {

    protected override void Awake() {
        base.Awake();
        GolemSavage golemSavage = baseObject as GolemSavage;
        golemSavage.OnPhaseTransition += GolemSavage_OnPhaseTransition;
    }

    private void GolemSavage_OnPhaseTransition(int maxHealth) {
        runtimeHP.UpdateMaxHealth(maxHealth);
    }

    protected override void BaseObject_OnTryDamage(int amount, ElementType element, EventResponse response) {
        if (!IFrameOn) {
            response.received = true;

            if (amount > 0) {
                runtimeHP.DoDamage(amount);
                baseObject.PropagateDamage(amount);

                StartCoroutine(ISimulateIFrame());
            }
        }
    }
}