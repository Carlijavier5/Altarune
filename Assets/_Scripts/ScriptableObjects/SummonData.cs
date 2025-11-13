using UnityEngine;

public abstract class SummonData : ScriptableObject {
    public SummonHologram prefabHologram;
    public Sprite icon;
    public int summonCost;
    public float manaDrain;
}

public abstract class SummonData<T> : SummonData where T : Summon {
    public T prefabSummon;
}