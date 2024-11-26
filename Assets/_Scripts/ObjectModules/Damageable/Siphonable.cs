using UnityEngine;

[RequireComponent(typeof(BaseObject))]
public class Siphonable : Damageable {

    protected override void BaseObject_OnTryDamage(int _, ElementType element,
                                                   EventResponse response) {
        response.received = true;
        int amount = !IFrameOn && element == ElementType.Siphon ? 1 : 0;

        if (amount > 0) {
            runtimeHP.DoDamage(amount);
            baseObject.PropagateDamage(amount);
            if (runtimeHP.Health <= 0) {
                baseObject.Perish();
                ToggleIFrame(true);
            }
        }
    }
}
