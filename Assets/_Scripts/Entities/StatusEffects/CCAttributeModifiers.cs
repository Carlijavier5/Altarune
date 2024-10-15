using UnityEngine;

[System.Serializable]
public class CCAttributeModifiers {

    public AttributeModifier ccRes = new(), stunRes = new(),
                             rootRes = new(), slowRes = new(),
                             massMod = new(), pushRes = new();

    public void ApplyModifiers(RuntimePushAttributes runtimeAttributes,
                               PushableAttributes defaultAttributes) {
        runtimeAttributes.objectMass = Mathf.Max(0.1f, defaultAttributes.objectMass * massMod);
        runtimeAttributes.pushResistance = Mathf.Clamp01(defaultAttributes.pushResistance * pushRes);
    }

    public void ApplyModifiers(RuntimeCCAttributes runtimeAttributes,
                               CCAttributes defaultAttributes) {
        runtimeAttributes.ccResistance = Mathf.Clamp01(defaultAttributes.ccResistance * ccRes);
        runtimeAttributes.stunResistance = Mathf.Clamp01(defaultAttributes.stunResistance * stunRes);
        runtimeAttributes.slowResistance = Mathf.Clamp01(defaultAttributes.rootResistance * rootRes);
        runtimeAttributes.slowResistance = Mathf.Clamp01(defaultAttributes.slowResistance * slowRes);
    }

    public void Compose(CCAttributeModifiers modifiers) {
        ccRes.Compose(modifiers.ccRes);
        stunRes.Compose(modifiers.stunRes);
        rootRes.Compose(modifiers.rootRes);
        slowRes.Compose(modifiers.slowRes);
        massMod.Compose(modifiers.massMod);
        pushRes.Compose(modifiers.pushRes);
    }
}