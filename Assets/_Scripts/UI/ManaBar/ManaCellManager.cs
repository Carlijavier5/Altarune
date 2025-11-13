using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaCellManager : MonoBehaviour {

    [SerializeField] private SummonController summonController;
    [SerializeField] private PingPongFadeAnimator fadeAnimator;
    [SerializeField] private ManaCell cellPrefab;

    private ManaCell[] cellArray;
    private int lastActiveIndex = -1;
    private int highlightCost = -1;

    public void Init(int cellAmount, int activeAmount) {
        cellArray = new ManaCell[cellAmount];
        float cellSpacing = 1f / cellAmount;

        for (int i = 0; i < cellAmount; i++) {
            ManaCell cell = Instantiate(cellPrefab, transform);
            cell.SetAnchors(i * cellSpacing, (i + 1) * cellSpacing);
            cellArray[i] = cell;
        }

        ActivateCells(activeAmount);

        summonController.OnSummonSelected += SummonController_OnSummonSelected;
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
        UpdateCellStatus();
    }

    private void DeactivateCell() {
        if (lastActiveIndex < 0) return;
        cellArray[lastActiveIndex].DoDischarge();
        lastActiveIndex = Mathf.Max(-1, lastActiveIndex - 1);
        UpdateCellStatus();
    }

    private void SummonController_OnSummonSelected(SummonType _, SummonData data) {
        highlightCost = data == null ? -1 : data.summonCost;
        UpdateCellStatus();
    }

    private void UpdateCellStatus() {
        DoOnCells((cell) => cell.SetOverlayStatus(ManaCellStatus.Standard));

        bool isManaSufficient = highlightCost <= (lastActiveIndex + 1);
        if (highlightCost > 0) {
            ManaCellStatus status = isManaSufficient ? ManaCellStatus.Commit : ManaCellStatus.Insufficient;

            int costProxy = highlightCost;
            for (int i = lastActiveIndex; i >= 0 && costProxy > 0; i--) {
                cellArray[i].SetOverlayStatus(status);
                costProxy--;
            }
        }

        fadeAnimator.DoFade(!isManaSufficient);
    }

    private void DoOnCells(System.Action<ManaCell> callback) {
        for (int i = 0; i < cellArray.Length; i++) {
            callback?.Invoke(cellArray[i]);
        }
    }
}