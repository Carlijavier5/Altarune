using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AggroRange : MonoBehaviour {

    public event System.Action<Entity> OnAggroEnter;
    public event System.Action<Entity> OnAggroExit;

    [SerializeField] private EntityFaction[] sensitiveFactions;
    public HashSet<Entity> AggroTargets { get; private set; } = new();

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)
            && sensitiveFactions.Any((faction) => faction == entity.Faction)) {
            if (AggroTargets.Add(entity)) {
                entity.OnPerish += Entity_OnPerish;
                OnAggroEnter?.Invoke(entity);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Entity entity)
            && sensitiveFactions.Any((faction) => faction == entity.Faction)) {
            TryRemoveEntity(entity);
        }
    }

    private void Entity_OnPerish(BaseObject baseObject) {
        Entity entity = baseObject as Entity;
        TryRemoveEntity(entity);
    }

    private void TryRemoveEntity(Entity entity) {
        if (AggroTargets.Remove(entity)) {
            entity.OnPerish -= Entity_OnPerish;
            OnAggroExit?.Invoke(entity);
        }
    }
}