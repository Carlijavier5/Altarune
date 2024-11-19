using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AggroRange : MonoBehaviour {

    private enum SortCriteria { Closest, Furthest }

    public event System.Action<Entity> OnAggroEnter;
    public event System.Action<Entity> OnAggroExit;

    [SerializeField] private EntityFaction[] sensitiveFactions;
    public HashSet<Entity> AggroTargets { get; private set; } = new();

    public Entity ClosestTarget => FindTarget(SortCriteria.Closest);
    public Entity FurthestTarget => FindTarget(SortCriteria.Furthest);

    private Entity FindTarget(SortCriteria criteria) {
        if (AggroTargets.Count == 0) return null;
        Entity optimalTarget = AggroTargets.First();
        float optimalDistance = Vector3.Distance(optimalTarget.transform.position,
                                                 transform.position);
        foreach (Entity target in AggroTargets) {
            float newDistance = Vector3.Distance(target.transform.position,
                                                 transform.position);
            if (criteria switch { SortCriteria.Furthest => newDistance > optimalDistance,
                                                      _ => newDistance < optimalDistance }) {
                optimalTarget = target;
                optimalDistance = newDistance;
            }
        }

        return optimalTarget;
    }

    void OnTriggerEnter(Collider other) {
        if (!transform.IsChildOf(other.transform)
            && other.TryGetComponent(out Entity entity)
            && sensitiveFactions.Any((faction) => faction == entity.Faction)) {
            if (AggroTargets.Add(entity)) {
                entity.OnPerish += Entity_OnPerish;
                OnAggroEnter?.Invoke(entity);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (!transform.IsChildOf(other.transform)
            && other.TryGetComponent(out Entity entity)
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

    public void Disable() => gameObject.SetActive(false);
}