using System.Collections;
using UnityEngine;

public class TowerPlague : Summon {

    [SerializeField] private PlagueArea plagueArea;
    [SerializeField] private Transform launchPoint;

    public override void Init(Entity summoner,
                              ManaSource manaSource) {
        base.Init(summoner, manaSource);
        SpawnPlagueArea();
    }

    protected override void Update() {
        if (!active) return;
        base.Update();
    }

    private void SpawnPlagueArea() {
        Instantiate(plagueArea, launchPoint.transform.position, Quaternion.identity);
    }
}