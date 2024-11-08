using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : Damageable {

    [SerializeField] private int doubleDamageThreshold;

    protected override void BaseObject_OnTryDamage(int amount, ElementType element, EventResponse response) {
        if (!IFrameOn) {
            response.received = true;

            if (amount > 0) {
                int processedAmount = amount > doubleDamageThreshold ? 2 : 1;
                runtimeHP.DoDamage(processedAmount);
                baseObject.PropagateDamage(processedAmount);
                StartCoroutine(ISimulateIFrame());

                if (runtimeHP.Health <= 0) {
                    baseObject.Perish();
                    ToggleIFrame(true);
                    PHGameManager.Instance.DoGameOver();
                }
            }
        }
    }

    protected override IEnumerator ISimulateIFrame() {
        localIFrameOn = true;
        baseObject.SetMaterial(iFrameProperties.settings.flashMaterial);
        Time.timeScale = 0.6f;
        yield return new WaitForSeconds(iFrameProperties.duration / 2);
        Time.timeScale = 1f;
        baseObject.ResetMaterials();
        yield return new WaitForSeconds(iFrameProperties.duration / 2);
        localIFrameOn = false;
    }
}