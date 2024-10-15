using System.Collections;
using System.Collections.Generic;
using FeatureSamples;
using UnityEngine;
using UnityEngine.AI;

public class WindArea : MonoBehaviour
{
    [SerializeField] private float _stunMultiplier = 2.0f;

    private List<Entity> entitiesNearby = new List<Entity>();
    private Vector3 enemyDirection;
    private bool isRunning = false;
  
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

    public bool GetRunning() {
        return isRunning;
    }

    private Vector3 EntityDirection(Entity entity){
        Vector3 entityPos = entity.transform.position;
        return new Vector3(entityPos.x - transform.position.x, 0, entityPos.z - transform.position.z );

    }

}


