using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NEED TO DECIDE: used isTrigger(). slows down standardBullets which impact FriendlyFireBullets(Unsure if exists, can't reproduce. Please notify if it occurs)
    //Establish how spawns will be handled. i.e. will the smallProjectiles be able to immediatly damage
            //Will there be a grace period. Will they spawn immediatly at the spawn position or will they slightly outward?
    //Finish smallProjectile behavior (Damage on enemies).
public class FriendlyFireBigProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    [SerializeField] private float moveForce;
    [SerializeField] private FriendlyFireSmallProjectile projectilePrefab;
    //Determines how many smallProjectiles are summoned upon death.
    [SerializeField] private int numClones;
    void Awake()
    {

    }

    //Method to allow to launching projectile in a specific direction
    public void Launch(Vector3 direction)
    {
        rb.AddForce(direction * moveForce);
    }
     //Used for detecting collision
     void OnTriggerEnter(Collider other)
    {
        if ((other is MeshCollider
            || other is BoxCollider
            || other is TerrainCollider)) {
            End(); 
        }
        if(other.TryGetComponent(out Entity entity)){
            entity.TryDamage(2);
            End();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void End()
    {
        Destroy(rb);
        Destroy(coll);
        //creates numClones FriendlyFireSmallProjectiles which are equally spaced.
        int degree = 0;
        for(int i = 0;i < numClones;i++)
        {
            FriendlyFireSmallProjectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Quaternion myRotation = Quaternion.AngleAxis(degree, Vector3.up);
            Vector3 startDirection = transform.right;
            Vector3 result = myRotation * startDirection;
            projectile.Launch(result);
            degree += 360 / numClones;
        }
        //Create and Instantiate FriendlyFireSmallProjectile
        
        Destroy(gameObject);
    }
}
