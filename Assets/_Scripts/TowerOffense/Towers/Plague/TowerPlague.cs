using System.Collections;
using UnityEngine;

public class TowerPlague : Summon {

    [SerializeField] private PlagueArea plagueArea;
    [SerializeField] private Transform launchPoint;
    private bool init;

    protected override void Awake() {
        base.Awake();
    }

    public override void Init(Player player) {
        base.Init(player);
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