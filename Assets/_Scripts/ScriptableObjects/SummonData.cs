using UnityEngine;

public abstract class SummonData : ScriptableObject {
    public SummonHologram prefabHologram;
    public Sprite icon;
    public float summonCost, manaDrain;
}

public abstract class SummonData<T> : SummonData where T : Summon {
    public T prefabSummon;
}