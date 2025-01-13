using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TeleportNexusInteractor : MonoBehaviour {
    
    [SerializeField] BoxCollider collider;
    [SerializeField] CinemachineVirtualCamera interactorCamera;
    
    
    
    Player interactingPlayer = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) {
            
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            Debug.Log("Player entered Nexus Teleporter");
            interactingPlayer = player;
            
            // Switch to camera
            interactorCamera.Priority = 99;
            
            // Disable player input
            GM.Player.InputSource.DeactivateInput();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            Debug.Log("Player exited Nexus Teleporter");
            interactingPlayer = null;
            
            // Switch out of camera
            interactorCamera.Priority = 0;
            
            // Enable player input
            GM.Player.InputSource.ActivateInput();
            
            //
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            Debug.Log("Player inside Nexus Teleporter");
        }
    }
    
    
}
