using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SandPit : MonoBehaviour
{
    /*
     Intended Behavior: A slow moving sandpit which applies a small pull effect into itself and deals damage to those in the center
     There are 3 major sections of the circle
     It should move very slowly. It does NOT do damage.
     Outer most: A minor pull towards the center which can be resisted.
     Middle most: A strong pull which is stronger than yan's running speed. Is outpaced by yan's dodge
     Center: Super strong pull which requires lots of dodging by Yan.
    */
    [SerializeField] private float suckUpdateTime;
    [SerializeField] private float suckSpeed;
    [SerializeField] private float suckScale;

    private Transform player;
    private float updateClock;
    private float suckUpdateClock;
    private HashSet<Entity> suckTargets = new();
    private NavMeshAgent navigation;
    private bool  firstDestination = false;


    // Start is called before the first frame update
    void Start()
    {
        navigation = GetComponent<NavMeshAgent>();
        navigation.speed = 4;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Suck();
    }
    
    private void Move(){
        if(!navigation.hasPath && !navigation.pathPending) {
            navigation.SetDestination(UnityEngine.Random.insideUnitSphere * 80);
        }
    }
    private void Suck(){
        suckUpdateClock += Time.deltaTime;
        if (suckUpdateClock > suckUpdateTime) {
            suckUpdateClock = 0;
            foreach (Entity i in suckTargets) {
                Vector3 distance = this.transform.position - player.transform.position;
                float strenth = suckScale / distance.magnitude;
                Vector3 difference = Vector3.Normalize(distance) * (suckSpeed * suckScale);
            
                player.position += difference;
                updateClock = 0;
            }
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.TryGetComponent(out Entity entity) && 
        entity.Faction == EntityFaction.Friendly){
            suckTargets.Add(entity);
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.TryGetComponent(out Entity entity) && 
        entity.Faction == EntityFaction.Friendly){
            suckTargets.Remove(entity);
        }
    }
}
