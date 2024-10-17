using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FriendlyFireBigProjectile : MonoBehaviour{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    [SerializeField] private float moveForce;
    [SerializeField] private FriendlyFireSmallProjectile projectilePrefab;
    [SerializeField] private int numClones;

    public void Launch(Vector3 direction){
        rb.AddForce(direction * moveForce);
    }
     void OnTriggerEnter(Collider other){
        if(other.TryGetComponent(out Entity entity)){
            entity.TryDamage(2);
            End();
        } else if(!other.isTrigger){
            End();
        }
    }
    void End(){
        Destroy(rb);
        Destroy(coll);
        int degree = 0;
        Vector3 startDirection = transform.right;
        for(int i = 0;i < numClones;i++)
        {
            FriendlyFireSmallProjectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Quaternion myRotation = Quaternion.AngleAxis(degree, Vector3.up);
            Vector3 result = myRotation * startDirection;
            projectile.Launch(result);
            degree += 360 / numClones;
        }
        
        Destroy(gameObject);
    }
}
