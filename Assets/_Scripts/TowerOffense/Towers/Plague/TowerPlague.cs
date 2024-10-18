using System.Collections;
using UnityEngine;

public class TowerPlague : Summon {

    [SerializeField] private PlagueArea plagueArea;
    [SerializeField] private Transform launchPoint;
    private bool init;

    protected override void Awake() {
        base.Awake();
    }

    public override void Init() {
        init = true;
        SpawnPlagueArea();
    }

    void Update() {
        if (!init) return;
    }

    private void SpawnPlagueArea() {
        Instantiate(plagueArea, launchPoint.transform.position, Quaternion.identity);
    }
}