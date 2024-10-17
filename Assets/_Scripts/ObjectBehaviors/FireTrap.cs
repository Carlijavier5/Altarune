using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    private float damageTimer = 0.0f;
    [SerializeField]
    private float waitTime = 1.0f;
    private HashSet<Entity> entitiesInTrap = new HashSet<Entity>();
    // Update is called once per frame
    [SerializeField]
    private EntityFaction faction = EntityFaction.Friendly;
    void Update() {
        if (entitiesInTrap.Count > 0) {
            damageTimer += Time.deltaTime; // Increment damageTimer if any entity is within FireTrap collider
        } else {
            damageTimer = 0.0f; // Reset damageTimer if there are no entitites
        }
        if (damageTimer >= waitTime) {
            foreach (Entity entity in entitiesInTrap) {
                if (entity.Faction == faction) {
                    entity.TryDamage(1);
                }
            }
            damageTimer = 0;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            entitiesInTrap.Add(entity); // Add entity to list that enters collision box
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
        entitiesInTrap.Remove(entity); // Remove entity from list that leaves collision box
        }
    }
}
