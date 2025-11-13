using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSource : MonoBehaviour {

    public event System.Action<EventResponse<float>> OnManaTax;
    public event System.Action OnManaStateUpdate;
    public event System.Action OnManaFillAction;
    public event System.Action<int> OnManaCellFill;
    public event System.Action OnManaDrainAction;
    public event System.Action<int> OnManaCellDrain;

    public event System.Action<bool> OnManaCollapse;

    public float MaxMana { get; private set; }

    private float mana;
    public float Mana {
        get => mana;
        private set {
            mana = Mathf.Clamp(value, 0, MaxMana);
            if (mana <= 0) OnManaCollapse?.Invoke(true);
        }
    }

    public int FullCells => (int) (mana / cellSize);
    public int MaxCells => (int) (MaxMana / cellSize);

    private readonly float cellSize = 20;

    private bool isActive;

    void Update() {
        if (isActive) {
            EventResponse<float> eRes = new();
            OnManaTax?.Invoke(eRes);
            float manaChange = eRes.objectReference;
            if (manaChange > 0) Drain(manaChange * Time.deltaTime, false);
            else Fill(manaChange * Time.deltaTime, false);
            OnManaStateUpdate?.Invoke();
        }
    }

    public void Init(float cellAmount) {
        MaxMana = cellAmount * cellSize;
        Mana = MaxMana;
        isActive = true;
    }

    public void DrainCells(int cellAmount) => Drain(cellAmount * cellSize);
    public void FillCells(int cellAmount) => Fill(cellAmount * cellSize);

    public void Drain(float amount, bool isAction = true) {
        int prevCells = FullCells;
        
        float absAmount = Mathf.Abs(amount);
        Mana -= absAmount;

        if (isAction && absAmount > 0) OnManaDrainAction?.Invoke();
        int cellsLost = Mathf.Abs(prevCells - FullCells);
        OnManaCellDrain?.Invoke(cellsLost);
    }

    public void Fill(float amount, bool isAction = true) {
        int prevCells = FullCells;

        float absAmount = Mathf.Abs(amount);
        Mana += absAmount;

        if (isAction && absAmount > 0) OnManaFillAction?.Invoke();
        int cellsGained = Mathf.Abs(FullCells - prevCells);
        OnManaCellFill?.Invoke(cellsGained);
    }

    public bool CanAfford(float amount) => mana > amount;

    public void TriggerManaCollapse(bool doVFX) => OnManaCollapse?.Invoke(doVFX);
}