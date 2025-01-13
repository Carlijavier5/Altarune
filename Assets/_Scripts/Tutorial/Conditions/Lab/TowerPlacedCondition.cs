using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacedCondition : CCondition {
    [SerializeField] private int towersRequired = 1;
    [SerializeField] private SummonType requiredSummon;
    private int towersPlaced = 0;
    private SummonType activeSummon;
    
    private void Start() {
        GM.Instance.OnPlayerInit += RegisterEvent;
    }
    
    private void RegisterEvent() {
        GM.Player.InputSource.OnSummonSelect += CheckTower;
        GM.Player.InputSource.OnSummonPerformed += TowerPlaceEvent;
    }

    private void CheckTower(SummonType summon, int index) {
        activeSummon = summon;
    }

    private void TowerPlaceEvent() {
        if (requiredSummon == activeSummon) {
            towersPlaced++;
            if (towersPlaced >= towersRequired) {
                CheckCondition();
            }
        }
    }
}
