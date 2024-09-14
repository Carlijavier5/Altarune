using UnityEngine;

public abstract class StatusEffect {

    public HealthAttributeModifiers AttributeMods { get; private set; }

    /// <summary>
    /// Called when the effect gets applied;
    /// </summary>
    public abstract void Apply(Entity entity);
    /// <summary>
    /// Called from the holding entity's update thread;
    /// </summary>
    /// <returns> True if the effect should be returned, false otherwise; </returns>
    public abstract bool Update(Entity entity);
    /// <summary>
    /// Called when the effect is removed;
    /// </summary>
    public abstract void Terminate(Entity entity);
}

[System.Serializable]
public class AttributeModifier {
    public float addMod = 0;
    public float multMod = 1;

    public static float operator *(float value, AttributeModifier modifier) 
        => Mathf.Clamp01(value * modifier.multMod + modifier.addMod);

    public void Compose(AttributeModifier modifier) {
        addMod += modifier.addMod;
        multMod *= modifier.multMod;
    }
}