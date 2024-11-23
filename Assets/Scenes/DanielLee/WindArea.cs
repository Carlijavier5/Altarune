using System.Collections;
using System.Collections.Generic;
using FeatureSamples;
using UnityEngine;
using UnityEngine.AI;

public class WindArea : MonoBehaviour
{
    [SerializeField] private float pullRadius;
    [SerializeField] private float pushRadius;
    [SerializeField] private float _stunMultiplier = 2.0f;
    private SphereCollider radiusColl;


    private List<Entity> entitiesNearby = new List<Entity>();
    private bool isRunning = false;

    private void Start() {
        radiusColl = GetComponent<SphereCollider>();
    }

    void OnTriggerEnter(Collider other){
        if (!other.TryGetComponent(out Entity entity)) return;
        if (entitiesNearby.Contains(entity)) return;
        if (entity.Faction != EntityFaction.Hostile) return;

        entitiesNearby.Add(entity);
        isRunning = true;
        //Debug.Log(entitiesNearby.Count);
    }

    void OnTriggerExit(Collider other){
        if (!other.TryGetComponent(out Entity entity)) return;
        if (!entitiesNearby.Contains(entity)) return;
        if (entity.Faction != EntityFaction.Hostile) return;

        entitiesNearby.Remove(entity);
        if (entitiesNearby.Count == 0) isRunning = false;
        //Debug.Log(entitiesNearby.Count);
    }
    public void PushNearby(float strength, float duration){
        foreach (Entity entity in entitiesNearby){
            if (entity) entity.ApplyEffects(new[] { new SampleStunEffect(duration * _stunMultiplier)});
            entity.TryLongPush(EntityDirection(entity), strength, duration,  out PushActionCore core);
            
        }
    }

    public void PullNearby(float pullspeed, float rotationspeed, float time) {
        Vector3 tangentPoint;
        Vector3 direction;
        foreach (Entity entity in entitiesNearby) {
            direction = -EntityDirection(entity);
            tangentPoint = new Vector3(entity.transform.position.x + -direction.z * 4, 0,
                entity.transform.position.z + direction.x * 4);
            entity.TryPush(EntityDirection(entity, tangentPoint), rotationspeed);
            if (direction.magnitude <= 1f) {
                if (entity) entity.ApplyEffects(new[] { new SampleStunEffect(time) });
                //Debug.Log("stunning");
                return;
            }
                entity.TryPush(direction, pullspeed);
            //Debug.Log("pulling");
        }
    }

    public void changeRadius(bool mode) {
        if (mode) {
            radiusColl.radius = pullRadius;
        }
        else radiusColl.radius = pushRadius;
    }

    public bool GetRunning() {
        return isRunning;
    }



    private Vector3 EntityDirection(Entity entity){
        Vector3 entityPos = entity.transform.position;
        return new Vector3(entityPos.x - transform.position.x, 0, entityPos.z - transform.position.z );
    }

    private Vector3 EntityDirection(Entity entity, Vector3 pullPoint) {
        Vector3 entityPos = entity.transform.position;
        return new Vector3(entityPos.x - pullPoint.x, 0, entityPos.z - pullPoint.z);
    }

}


