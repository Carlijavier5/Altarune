using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FriendlyFireSmallProjectile : MonoBehaviour{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    [SerializeField] private float moveForce;
    [SerializeField] private float startCollisionTime;
    [SerializeField] private MeshRenderer rend;
    private float time;
    void Start(){
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            rend.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", Color.white);
            rend.SetPropertyBlock(propertyBlock);
    }
    void Update(){
        updateTime();
        
    }
    private void updateTime(){
        time += Time.deltaTime;
        if(time >= startCollisionTime){
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            rend.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", Color.grey);
            rend.SetPropertyBlock(propertyBlock);
        }
    }
    public void Launch(Vector3 direction){
        rb.AddForce(direction * moveForce);
    }
    void OnTriggerEnter(Collider other){
        if(time >= startCollisionTime){
            if(other.TryGetComponent(out Entity entity)){
                entity.TryDamage(1);
                End();
            } else if(!other.isTrigger){
                End();
            }
        }
    }
    void End(){
        Destroy(gameObject);
    }
}
