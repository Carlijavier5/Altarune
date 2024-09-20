using System.Collections.Generic;
using UnityEngine;

public class Battery : Summon {

    [SerializeField] private GameObject areaIndicator;

    private List<Summon> linkedTowers = new();

    public void LinkTower(Summon tower)
    {
        linkedTowers.Add(tower);
    }

    public override void Init() { }

    public void ToggleArea(bool on) {
        areaIndicator.SetActive(on);
    }
}