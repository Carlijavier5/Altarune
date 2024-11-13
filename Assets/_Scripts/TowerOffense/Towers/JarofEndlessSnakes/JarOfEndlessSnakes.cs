using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random=UnityEngine.Random;
public class JarOfEndlessSnakes : Summon
{
    [SerializeField] private int maxSnakes;
    [SerializeField] private float summonCooldown;
    [SerializeField] private Snake snakePrefab;
    private GameObject player = null;
    private float timeToNextSummon = 0;
    private List<Snake> snakes = new List<Snake>();

    public override void Init(Entity summoner,
                              ManaSource manaSource) {
        base.Init(summoner, manaSource);
        timeToNextSummon = summonCooldown;
    }
    
    void Awake() {
        if (player == null){
            player = FindObjectOfType<PlayerController>().gameObject;
        }
    }

    // Update is called once per frame
    protected override void Update(){
        if(!active) return;
        base.Update();
        timeToNextSummon -= Time.deltaTime;
        if(snakes.Count == maxSnakes){
            timeToNextSummon = summonCooldown;
        }
        if (timeToNextSummon < 0) {
            summonSnake();
            timeToNextSummon = summonCooldown;
        }
    }

    private void summonSnake(){
        Snake summoned = Instantiate(snakePrefab, transform.position, Quaternion.identity);
        float angle = Random.Range(0, 2* (float) Math.PI);
        summoned.transform.Translate(new Vector3(1.5f * (float) Math.Cos(angle), 0, 1.5f * (float) Math.Sin(angle)));
        summoned.parent = this;
        summoned.id = snakes.Count;
        snakes.Add(summoned);
    }

    public void deleteSnake(int index){
        Destroy(snakes[index]);
        snakes.RemoveAt(index);
    }

    public void OnDestroy(){
        foreach(Snake s in snakes){
            Destroy(s);
        }
        snakes.Clear();
    }
}
