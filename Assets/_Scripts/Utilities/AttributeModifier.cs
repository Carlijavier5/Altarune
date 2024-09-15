[System.Serializable]
public class AttributeModifier {
    public float addMod = 0;
    public float multMod = 1;

    public static float operator *(float value, AttributeModifier modifier) 
        => value * modifier.multMod + modifier.addMod;

    public void Compose(AttributeModifier modifier) {
        addMod += modifier.addMod;
        multMod *= modifier.multMod;
    }
}