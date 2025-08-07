using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarNew : MonoBehaviour {
    [SerializeField] private ManaSource manaSource;
    [SerializeField] private ManaCellManager cellManager;
    [SerializeField] private Image topLayer, interpolationLayer;

    private float Mana => manaSource.Mana;
    private float MaxMana => manaSource.MaxMana;
    private float FillAmount => MaxMana > 0 ? Mana / MaxMana : 0;

    void Start() {
        cellManager.Init(manaSource.MaxCells, manaSource.FullCells);
        manaSource.OnManaFill += ManaSource_OnManaFill;
        manaSource.OnManaDrain += ManaSource_OnManaDrain;
    }

    void Update() {
        topLayer.fillAmount = Mathf.MoveTowards(topLayer.fillAmount,
                                                FillAmount, Time.deltaTime);
        interpolationLayer.fillAmount = Mathf.MoveTowards(interpolationLayer.fillAmount,
                                                          FillAmount, Time.deltaTime);
    }

    private void ManaSource_OnManaFill(int cellsGained) {
        cellManager.ActivateCells(cellsGained);
        interpolationLayer.fillAmount = Mana / MaxMana;
    }

    private void ManaSource_OnManaDrain(int cellsLost) {
        cellManager.DeactivateCells(cellsLost);
        topLayer.fillAmount = Mana / MaxMana;
    }
}
