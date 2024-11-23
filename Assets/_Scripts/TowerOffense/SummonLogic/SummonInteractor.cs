using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonInteractor : MonoBehaviour {

    [SerializeField] private SummonController inputSource;

    void Awake() {
        inputSource.OnSummonSelected += InputSource_OnSummonSelected;
        inputSource.OnRaycastUpdate += InputSource_OnRaycastUpdate;
        inputSource.OnPointerConfirm += InputSource_OnPointerConfirm;
        inputSource.ManaSource.OnManaCollapse += Player_OnManaCollapse;
    }

    private void InputSource_OnSummonSelected(SummonType summonType, SummonData _) {
        if (summonType != 0) {
            /// Disable active HUDs;
        }
    }

    private void InputSource_OnRaycastUpdate(Ray ray) {
        if (Physics.Raycast(ray, out RaycastHit unitHit, LayerUtils.MAX_RCD, LayerUtils.BaseObjectLayerMask)) {

        }
    }

    private void InputSource_OnPointerConfirm(SummonType s) {

    }

    private void Player_OnManaCollapse() {

    }
}