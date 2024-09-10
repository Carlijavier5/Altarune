public abstract class StatusEffect {
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