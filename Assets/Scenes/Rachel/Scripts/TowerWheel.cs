using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerWheel : MonoBehaviour
{
    [SerializeField] private SummonController summonController;
    [SerializeField] private Animator animator;

    private void Start() {
        summonController.OnSummonSelected += HandleSummonSelected;
    }

    private void OnEnable() {
        if (summonController != null) {
            summonController.OnSummonSelected += HandleSummonSelected;
        }
    }

    private void OnDisable() {
        if (summonController != null) {
            summonController.OnSummonSelected -= HandleSummonSelected;
        }
    }

    private void HandleSummonSelected(SummonType type, SummonData data) {
        if (!data) {
            Hide();
            return;
        }

        animator.SetBool("isSelect", true);
    }

    private void Hide() {
        animator.SetBool("isSelect", false);
    }
}
