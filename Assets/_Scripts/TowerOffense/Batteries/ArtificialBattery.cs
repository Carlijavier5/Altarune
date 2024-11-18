using System.Collections.Generic;
using UnityEngine;

public class ArtificialBattery : Summon, IBattery {

    [SerializeField] private GameObject areaIndicator;
    private readonly HashSet<Summon> linkedTowers = new();

    public ManaSource ManaSource => manaSource;
    public Vector3 Position => transform.position;
    public bool IsActive => active;

    public void LinkTower(Summon tower) => linkedTowers.Add(tower);

    public override void Collapse() {
        foreach (Summon tower in linkedTowers) {
            tower.Collapse();
        } base.Collapse();
    }

    public void ToggleArea(bool on) {
        areaIndicator.SetActive(on);
    }
}