using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(GolemSiftling))]
public class SiftlingDamageable : Damageable {

    protected override void Awake() {
        base.Awake();
        GolemSiftling golemSiftling = baseObject as GolemSiftling;
        golemSiftling.OnAscend += GolemSavage_OnAscend;
    }

    private void GolemSavage_OnAscend(int maxHealth) {
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