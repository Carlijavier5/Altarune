using System.Collections;
using UnityEngine;

public class TowerPlague : Summon {

    [SerializeField] private PlagueArea plagueArea;
    [SerializeField] private Transform launchPoint;
    private bool init;

    public override void Init(ManaSource manaSource) {
        base.Init(manaSource);
        init = true;
        SpawnPlagueArea();
    }

    protected override void Update() {
        if (!init) return;
        base.Update();
    }

    private void SpawnPlagueArea() {
        Instantiate(plagueArea, launchPoint.transform.position, Quaternion.identity);
    }
}