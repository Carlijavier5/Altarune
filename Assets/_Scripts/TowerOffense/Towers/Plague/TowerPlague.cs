using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlague : Summon {

    [SerializeField] private PlagueArea plagueArea;
    [SerializeField] private float launchInterval;
    private float timer;

    [Header("Jar of Endless Snakes Behavior")]
    [SerializeField] private int maxSnakes;
    [SerializeField] private float summonCooldown;
    [SerializeField] private Snake snakePrefab;
    private float timeToNextSummon = 0;
    private readonly List<Snake> snakes = new();

    void Awake() {
        plagueArea.EntityID = GetInstanceID();
        timeToNextSummon = summonCooldown;
    }

    void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            timer = launchInterval;
            plagueArea.DoWave();
        }

        // Jar of Endless Snakes
        timeToNextSummon -= Time.deltaTime;
        if (snakes.Count == maxSnakes) {
            timeToNextSummon = summonCooldown;
        }
        if (timeToNextSummon < 0) {
            SummonSnake();
            timeToNextSummon = summonCooldown;
        }
    }

    /// Jar of Endless Snakes
    private void SummonSnake(){
        Snake summoned = Instantiate(snakePrefab, transform.position, Quaternion.identity);
        float angle = UnityEngine.Random.Range(0, 2 * (float) Math.PI);
        summoned.transform.Translate(new Vector3(1.5f * (float) Math.Cos(angle), 0, 1.5f * (float) Math.Sin(angle)));
        summoned.parent = this;
        summoned.id = snakes.Count;
        snakes.Add(summoned);
    }

    public void DeleteSnake(int index) {
        snakes.RemoveAt(index);
    }

    public override void Collapse() {
        base.Collapse();
        foreach (Snake s in snakes) {
            s.Perish(false);
        } snakes.Clear();
    }
}