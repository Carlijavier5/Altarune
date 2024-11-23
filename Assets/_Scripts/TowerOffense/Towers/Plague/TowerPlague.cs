using System;
using System.Collections;
using UnityEngine;

public class TowerPlague : Summon {

    [SerializeField] private PlagueArea plagueArea;
    [SerializeField] private Transform launchPoint;

    [SerializeField] bool debug;

    void Start() {
        if (debug) {
            Init(null, null);
        }
    }


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