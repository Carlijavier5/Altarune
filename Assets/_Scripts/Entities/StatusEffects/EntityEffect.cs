using UnityEngine;

public abstract class EntityEffect : StatusEffect<Entity> {

    public HealthAttributeModifiers HealthModifiers { get; protected set; }
    public CCAttributeModifiers CCModifiers { get; protected set; }
    public CrowdControlEffects CCEffects { get; protected set; }
}

public abstract class StatusEffect<T> where T : BaseObject {

    /// <summary>
    /// Called when the effect gets applied;
    /// </summary>
    public abstract void Apply(T entity, bool isNew);
    /// <summary>
    /// Called after the effect has been applied, but before the first frame of update;
    /// </summary>
    public virtual void Start(T entity) { }
    /// <summary>
    /// Called from the holding entity's update thread;
    /// </summary>
    /// <returns> True if the effect should be removed, false otherwise; </returns>
    public abstract bool Update(T entity);
    /// <summary>
    /// Called when the effect is removed;
    /// </summary>
    public abstract void Terminate(T entity);

    public StatusEffect<T> Clone() => MemberwiseClone() as StatusEffect<T>;

    public override int GetHashCode() => GetType().FullName.GetHashCode();
}