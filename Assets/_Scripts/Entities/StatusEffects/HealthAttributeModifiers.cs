﻿using UnityEngine;

[System.Serializable]
public class HealthAttributeModifiers {
    public AttributeModifier defense = new(), fireRes = new(),
                             iceRes = new(), shockRes = new(),
                             poisonRes = new(), healMod = new();

    public void ApplyModifiers(RuntimeHealthAttributes runtimeAttributes,
                               HealthAttributes defaultAttributes) {
        runtimeAttributes.defense = Mathf.Clamp01(defaultAttributes.defense * defense);
        runtimeAttributes.fireRes = Mathf.Clamp01(defaultAttributes.fireRes * fireRes);
        runtimeAttributes.iceRes = Mathf.Clamp01(defaultAttributes.iceRes * iceRes);
        runtimeAttributes.shockRes = Mathf.Clamp01(defaultAttributes.shockRes * shockRes);
        runtimeAttributes.poisonRes = Mathf.Clamp01(defaultAttributes.poisonRes * poisonRes);
        runtimeAttributes.healModifier = Mathf.Clamp01(defaultAttributes.healModifier * healMod);
    }

    public void Compose(HealthAttributeModifiers modifiers) {
        defense.Compose(modifiers.defense);
        fireRes.Compose(modifiers.fireRes);
        iceRes.Compose(modifiers.iceRes);
        shockRes.Compose(modifiers.shockRes);
        poisonRes.Compose(modifiers.poisonRes);
        healMod.Compose(modifiers.healMod);
    }
}