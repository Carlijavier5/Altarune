using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyFireSmallProjectile : MonoBehaviour{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    [SerializeField] private float moveForce;
    [SerializeField] private float startCollisionTime;
    private float time;
    void Start(){
        GetComponent<Renderer>().material.color = Color.white;
    }
    void Update(){
        updateTime();
    }
    private void updateTime(){
        time += Time.deltaTime;
        if(time >= startCollisionTime){
            GetComponent<Renderer>().material.color = Color.gray;
        }
    }
    public void Launch(Vector3 direction){
        rb.AddForce(direction * moveForce);
    }
    //Detects collisions
    void OnTriggerEnter(Collider other){
        if(time >= startCollisionTime){
            if ((other is MeshCollider
                || other is BoxCollider
                || other is TerrainCollider)) {
                End();
            }
            if(other.TryGetComponent(out Entity entity)){
                entity.TryDamage(1);
                End();
            }
        }
    }
    void End(){
        Destroy(gameObject);
    }
}
