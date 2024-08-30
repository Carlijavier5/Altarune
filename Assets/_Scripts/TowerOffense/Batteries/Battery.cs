using System.Collections.Generic;
using UnityEngine;

public class Battery : Summon {

    [SerializeField] private GameObject areaIndicator;
    private List<SampleProjectileTower> linkedTowers = new();

    public void LinkTower(SampleProjectileTower tower) => linkedTowers.Add(tower);

    public override void Init() { }

    public void ToggleArea(bool on) {
        areaIndicator.SetActive(on);
    }
}