using UnityEngine;

[System.Serializable]
public class HealthAttributeModifiers {
    public AttributeModifier defense = new(), healMod = new();

    public void ApplyModifiers(RuntimeHealthAttributes runtimeAttributes,
                               HealthAttributes defaultAttributes) {
        runtimeAttributes.defense = Mathf.Clamp01(defaultAttributes.defense * defense);
        runtimeAttributes.healModifier = Mathf.Clamp01(defaultAttributes.healModifier * healMod);
    }

    public void Compose(HealthAttributeModifiers modifiers) {
        defense.Compose(modifiers.defense);
        healMod.Compose(modifiers.healMod);
    }
}