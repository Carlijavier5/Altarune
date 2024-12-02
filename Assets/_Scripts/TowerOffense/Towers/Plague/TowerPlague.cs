using System;
using System.Collections;
using UnityEngine;

public class TowerPlague : Summon {

    [SerializeField] private PlagueArea plagueArea;
    [SerializeField] private float launchInterval;
    private float timer;

    void Awake() {
        plagueArea.EntityID = GetInstanceID();
    }

    void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            timer = launchInterval;
            plagueArea.DoWave();
        }
    }
}