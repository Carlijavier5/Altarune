using System.Collections.Generic;
using UnityEngine;

public class Battery : Summon {

    [SerializeField] private GameObject areaIndicator;
    private List<ProjectileTower> linkedTowers;

    public void LinkTower(ProjectileTower tower) => linkedTowers.Add(tower);

    public void ToggleArea(bool on) {
        areaIndicator.SetActive(on);
    }

    public void HalfFade() {
        SwapFade(true);
    }
}