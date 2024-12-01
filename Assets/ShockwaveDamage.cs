using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveDamage : Entity {
    public void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)
            && entity.Faction != EntityFaction.Hostile) {

            Collider entityCollider = other.GetComponent<Collider>();
            if (entityCollider != null) {
                // Gets the closest point of the collider
                Vector3 closestPoint = entityCollider.ClosestPoint(other.transform.position);

                // Finds the distance between the edge of the collider and the entity
                float distanceToCollider = Vector3.Distance(other.transform.position, closestPoint);

                if (distanceToCollider <= 5f) {
                    bool isDamageable = entity.TryDamage(3);
                }
            }
        }
    }
}
