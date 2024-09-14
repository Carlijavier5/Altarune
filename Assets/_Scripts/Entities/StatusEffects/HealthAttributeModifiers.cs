[System.Serializable]
public class HealthAttributeModifiers {
    public AttributeModifier defense, fireRes, iceRes,
                             poisonRes, healMod;

    public void ApplyModifiers(RuntimeHealthAttributes runtimeAttributes,
                               HealthAttributes defaultAttributes) {
        runtimeAttributes.defense = defaultAttributes.defense * defense;
        runtimeAttributes.fireRes = defaultAttributes.fireRes * fireRes;
        runtimeAttributes.iceRes = defaultAttributes.iceRes * iceRes;
        runtimeAttributes.poisonRes = defaultAttributes.poisonRes * poisonRes;
        runtimeAttributes.healModifier = defaultAttributes.healModifier * healMod;
    }

    public void Compose(HealthAttributeModifiers modifiers) {
        defense.Compose(modifiers.defense);
        fireRes.Compose(modifiers.fireRes);
        iceRes.Compose(modifiers.iceRes);
        poisonRes.Compose(modifiers.poisonRes);
        healMod.Compose(modifiers.healMod);
    }
}
