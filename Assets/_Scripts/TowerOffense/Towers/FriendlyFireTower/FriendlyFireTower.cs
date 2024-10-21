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
    private bool init;

    protected override void Awake(){
        base.Awake();
    }

    public override void Init(Player player) {
        base.Init(player);
        init = true;
    }

    // Update is called once per frame
    protected override void Update(){
        Target();
        base.Update();
    }
    /// <summary>
    /// Calculates the closest entity within it's aggro range and launches a FriendlyFireBigProjectile toward the entity with a random spread
    /// </summary>
    private void Target(){
        Vector3 closestVector = new Vector3(float.MaxValue, float.MaxValue, Int32.MaxValue);
        if(!init) return;
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
