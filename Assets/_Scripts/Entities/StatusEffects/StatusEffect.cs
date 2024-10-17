using UnityEngine;

public abstract class StatusEffect {

    public HealthAttributeModifiers HealthModifiers { get; protected set; }
    public CCAttributeModifiers CCModifiers { get; protected set; }
    public CrowdControlEffects CCEffects { get; protected set; }

    /// <summary>
    /// Called when the effect gets applied;
    /// </summary>
    public abstract void Apply(Entity entity, bool isNew);
    /// <summary>
    /// Called after the effect has been applied, but before the first frame of update;
    /// </summary>
    public virtual void Start(Entity entity) { }
    /// <summary>
    /// Called from the holding entity's update thread;
    /// </summary>
    /// <returns> True if the effect should be removed, false otherwise; </returns>
    public abstract bool Update(Entity entity);
    /// <summary>
    /// Called when the effect is removed;
    /// </summary>
    public abstract void Terminate(Entity entity);

    public StatusEffect Clone() => MemberwiseClone() as StatusEffect;
}