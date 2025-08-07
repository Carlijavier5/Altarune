using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaCellManager : MonoBehaviour {

    [SerializeField] private ManaCell cellPrefab;

    private ManaCell[] cellArray;
    private int lastActiveIndex = -1;

    public void Init(int cellAmount, int activeAmount) {
        cellArray = new ManaCell[cellAmount];
        float cellSpacing = 1f / cellAmount;

        for (int i = 0; i < cellAmount; i++) {
            ManaCell cell = Instantiate(cellPrefab, transform);
            cell.SetAnchors(i * cellSpacing, (i + 1) * cellSpacing);
            cellArray[i] = cell;
        }

        ActivateCells(activeAmount);
    }

    public void ActivateCells(int cellAmount) {
        for (int i = 0; i < cellAmount; i++) ActivateCell();
    }

    public void DeactivateCells(int cellAmount) {
        for (int i = 0; i < cellAmount; i++) DeactivateCell();
    }

    private void ActivateCell() {
        lastActiveIndex = Mathf.Min(cellArray.Length - 1, lastActiveIndex + 1);
        cellArray[lastActiveIndex].DoCharge();
    }

    private void DeactivateCell() {
        if (lastActiveIndex < 0) return;
        cellArray[lastActiveIndex].DoDischarge();
        lastActiveIndex = Mathf.Max(-1, lastActiveIndex - 1);
    }
}