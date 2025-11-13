using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class TriggerDamageEffector : MonoBehaviour
{
    [SerializeField] private EntityFaction[] applicableFactions;
    [SerializeField] private int damageAmount;

    private readonly Dictionary<Collider, Entity> stayEntityMap = new();

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)
                && applicableFactions.Contains(entity.Faction)) {
            entity.TryDamage(damageAmount);
            stayEntityMap.TryAdd(other, entity);
        }
    }

    void OnTriggerStay(Collider other) {
        if (stayEntityMap.TryGetValue(other, out Entity entity)) {
            entity.TryDamage(damageAmount);
        }
    }

    void OnTriggerExit(Collider other) {
        stayEntityMap.Remove(other);
    }
}
