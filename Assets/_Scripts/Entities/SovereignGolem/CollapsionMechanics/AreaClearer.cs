using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AreaClearer : MonoBehaviour {

    [SerializeField] private Transform playerFallback;
    [SerializeField] private NavMeshObstacle navMeshObstacle;
    [SerializeField] private float clearRadius;

    public void ClearArea() {
        Collider[] entitiesInRadius = Physics.OverlapSphere(transform.position, clearRadius,
                                                            LayerUtils.EntityLayerMask);
        foreach (Collider collider in entitiesInRadius) {
            if (collider.TryGetComponent(out Entity entity)) {
                if (entity is Player) {
                    Player player = entity as Player;
                    player.TryTeleport(playerFallback.position);
                } else {
                    entity.Perish();
                }
            }
        }

        navMeshObstacle.gameObject.SetActive(true);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, clearRadius);
    }
    #endif
}