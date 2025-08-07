using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSource : MonoBehaviour {

    public event System.Action<EventResponse<float>> OnManaTax;
    public event System.Action<int> OnManaFill;
    public event System.Action<int> OnManaDrain;

    public event System.Action OnManaCollapse;

    private bool active;

    public float MaxMana { get; private set; }

    private float mana;
    public float Mana {
        get => mana;
        private set {
            mana = Mathf.Clamp(value, 0, MaxMana);
            if (mana <= 0) OnManaCollapse?.Invoke();
        }
    }

    public int FullCells => (int) (mana / cellSize);
    public int MaxCells => (int) (MaxMana / cellSize);

    private readonly float cellSize = 20;

    void Update() {
        if (active) {
            EventResponse<float> eRes = new();
            OnManaTax?.Invoke(eRes);
            float manaDrain = eRes.objectReference;
        }
    }

    public void Init(float cellAmount) {
        MaxMana = cellAmount * cellSize;
        Mana = MaxMana;
        active = true;
    }

    public void DrainCells(int cellAmount) => Drain(cellAmount * cellSize);
    public void FillCells(int cellAmount) => Fill(cellAmount * cellSize);

    public void Drain(float amount) {
        int prevCells = FullCells;
        
        float absAmount = Mathf.Abs(amount);
        Mana -= absAmount;

        int cellsLost = Mathf.Abs(prevCells - FullCells);
        OnManaDrain?.Invoke(cellsLost);
    }

    public void Fill(float amount) {
        int prevCells = FullCells;

        float absAmount = Mathf.Abs(amount);
        Mana += absAmount;

        int cellsGained = Mathf.Abs(FullCells - prevCells);
        OnManaFill?.Invoke(cellsGained);
    }

    public bool CanAfford(float amount) => mana > amount;
}