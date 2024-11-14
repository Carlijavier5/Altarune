using UnityEngine;

public abstract class SummonData<T> : ScriptableObject where T : Summon {
    public T prefabSummon;
    public SummonHologram prefabHologram;
    public Sprite icon;
    public float summonCost;
}