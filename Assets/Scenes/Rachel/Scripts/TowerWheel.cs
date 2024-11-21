using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerWheel : MonoBehaviour
{
    [SerializeField] private SummonController summonController;
    
    private Animator animator;

    private bool didSelect = true;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

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
            animator.SetBool("isSelect", true);
        }

        didSelect = true;
    }

    private void HandleBatterySelected(BatteryData batteryData) {
        if (didSelect) {
            animator.SetBool("isSelect", true);
        }

        didSelect = true;
    }

    private void HandleDeselect() {
        animator.SetBool("isSelect", false);
        didSelect = false;
    }
}
