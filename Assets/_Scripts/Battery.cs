using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour {

    [SerializeField] private GameObject areaIndicator;
    private List<Tower> linkedTowers;

    public void LinkTower(Tower tower) => linkedTowers.Add(tower);

    public void ToggleArea(bool on) {
        areaIndicator.SetActive(on);
    }

    public void HalfFade() {

    }
}