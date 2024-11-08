using System.Collections.Generic;
using UnityEngine;

public class Battery : Summon {

    [SerializeField] private GameObject areaIndicator;
    private HashSet<Summon> linkedTowers = new();

    public void LinkTower(Summon tower) => linkedTowers.Add(tower);

    public override void Init(Player player) {
        base.Init(player);
    }

    public override void Collapse() {
        foreach (Summon tower in linkedTowers) {
            tower.Collapse();
        } base.Collapse();
    }

    public void ToggleArea(bool on) {
        areaIndicator.SetActive(on);
    }
}