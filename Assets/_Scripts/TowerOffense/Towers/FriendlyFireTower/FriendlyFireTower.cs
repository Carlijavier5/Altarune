using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class FriendlyFireTower : Summon{
    [SerializeField] private FriendlyFireBigProjectile projectilePrefab;
    [SerializeField] private float attackInterval;
    [SerializeField] private int minRandAngle;
    [SerializeField] private int maxRandAngle;
    [SerializeField] private AggroRange aggroRange;

    private float attackTick = -0.2f;

    // Update is called once per frame
    void Update(){
        Target();
    }
    /// <summary>
    /// Calculates the closest entity within it's aggro range and launches a FriendlyFireBigProjectile toward the entity with a random spread
    /// </summary>
    private void Target(){
        if (!active) return;
        Vector3 closestVector = new (float.MaxValue, float.MaxValue, int.MaxValue);
        attackTick += Time.deltaTime;
        if (attackTick >= attackInterval && aggroRange.AggroTargets.Count != 0) {
            foreach(Entity i in aggroRange.AggroTargets){
                Vector3 currVector = i.transform.position - this.transform.position;
                if(currVector.magnitude <= closestVector.magnitude) {
                    closestVector = currVector;
                }
            }
            System.Random rand = new System.Random();
            float angle = Vector3.SignedAngle(closestVector, transform.right, Vector3.up);
            float randomAngle = rand.Next(minRandAngle,maxRandAngle);
            FriendlyFireBigProjectile projectile = Instantiate(projectilePrefab, transform.position + Vector3.up, Quaternion.identity);
            Quaternion myRotation = Quaternion.AngleAxis(-angle + randomAngle, Vector3.up);
            Vector3 startDirection = transform.right;
            Vector3 result = myRotation * startDirection;
            projectile.Launch(result);
            attackTick = 0;
        }
    }
}
