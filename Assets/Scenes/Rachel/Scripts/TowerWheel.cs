using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerWheel : MonoBehaviour
{
    [SerializeField] private SummonController summonController;

    private bool didSelect = true;

    private void OnEnable() {
        if (summonController != null) {
            summonController.OnTowerSelected += HandleTowerSelected;
            summonController.OnBatterySelected += HandleBatterySelected;
            summonController.OnDeselect += HandleDeselect;
        }
    }

    private void OnDisable() {
        if (summonController != null) {
            summonController.OnTowerSelected -= HandleTowerSelected;
            summonController.OnBatterySelected -= HandleBatterySelected;
            summonController.OnDeselect -= HandleDeselect;
        }
    }

    private void HandleTowerSelected(TowerData towerData) {
        if (didSelect) {
            Debug.Log(towerData.name);
        }

        didSelect = true;
    }

    private void HandleBatterySelected(BatteryData batteryData) {
        if (didSelect) {
            Debug.Log(batteryData.name);
        }

        didSelect = true;
    }

    private void HandleDeselect() {
        Debug.Log("none");

        didSelect = false;
    }
}
