using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class TowerSwitcheroo : Summon {
    //For the transformations
    //[SerializeField] private Transform playerPosition, towerPosition;
    [SerializeField] private Vector3 playerPosition, towerPosition;
    [SerializeField] private GameObject tower;
    private Vector3 playerOriginal;

    //For SphereCast
    [SerializeField] private float radius;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask layerMask;

    RaycastHit enemiesHit;

    void Update() {
        if (active) {
            OnMouseUpAsButton();
        }
    }

    private void OnMouseUpAsButton() {
        //Attach enemy to tower
        CastSphere();

        //Swap!
        playerPosition = Summoner.transform.position;
        towerPosition = tower.transform.position;
        playerOriginal = playerPosition;

        Vector3 transformation = playerPosition - towerPosition;

        Summoner.TryTeleport(towerPosition);
        this.TryTeleport(playerOriginal);
        
        ArrayList allEnemies = FindEnemies(towerPosition, 15);
        foreach (Entity enemies in allEnemies) {
            enemies.TryTeleport(enemies.transform.position + transformation);
        }

        Collapse();
    }

    private ArrayList FindEnemies(Vector3 center, float radius) {
        Collider[] hitcolliders = Physics.OverlapSphere(center, radius, 1 << 0);
        //Collider itemFound = hitcolliders[0];
        ArrayList allEnemies = new ArrayList();
        foreach (var hitCollider in hitcolliders) {
            if (hitCollider.TryGetComponent(out Entity entities)) {
                allEnemies.Add(hitCollider);
            }
        }

        return allEnemies;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position - transform.up * maxDistance, radius);
    }

    private void CastSphere() {
        if (Physics.SphereCast(transform.position, radius, -transform.up, out enemiesHit, maxDistance, ~layerMask)) {
            if (enemiesHit.transform.TryGetComponent(out GolemSentinel Golem_Idle)) {
                Debug.Log(enemiesHit.collider.gameObject);
            }
        }
    }
}