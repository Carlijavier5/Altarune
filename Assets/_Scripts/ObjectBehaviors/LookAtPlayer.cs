using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour {

    [SerializeField] float turnSpeed = 4;
    
    void Start()
    {
        
    }

    void Update() {
        
        Vector3 origin = transform.position;
        Vector3 lookDestination = GM.Player.transform.position;
        Vector3 directionToPlayer = (lookDestination - origin).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

}
