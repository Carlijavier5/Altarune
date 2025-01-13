using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TeleportNexusInteractor : MonoBehaviour {
    
    [Header("Interaction")]
    [SerializeField] BoxCollider collider;
    [SerializeField] CinemachineVirtualCamera interactorCamera;

    Canvas canvas;
    
    Player interactingPlayer = null;

    [Space] [Header("Teleport")]
    [SerializeField] Transform buttonsContainer;
    [SerializeField] TeleportNexusUIButton uiButtonPrefab;
    [SerializeField] List<string> teleportEntries = new List<string>();

    List<TeleportNexusUIButton> uiButtonInstances;

    [Space] [Header("Display Settings")]
    [SerializeField] float showDelayBetweenButtons = 0.2f;

    [SerializeField] float hideDelayBetweenButtons = 0.1f;
    
    void Start() {
        if (!uiButtonPrefab) {
            Debug.LogError("[Teleport Nexus Interactor] uiButtonPrefab is unset or missing!!");
            return;
        }
        
        uiButtonInstances = new List<TeleportNexusUIButton>();
        for (int i = 0; i < teleportEntries.Count; i++) {
            TeleportNexusUIButton newButton = Instantiate(uiButtonPrefab, buttonsContainer);
            newButton.SetButtonLabel(teleportEntries[i]);
            uiButtonInstances.Add(newButton);
        }

        canvas = GetComponentInChildren<Canvas>();
        if (canvas) {
            canvas.worldCamera = Camera.main;
        }
    }

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
            //GM.Player.InputSource.DeactivateInput();
            
            // Show buttons
            StopAllCoroutines();
            StartCoroutine(ShowButtons());
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            Debug.Log("Player exited Nexus Teleporter");
            interactingPlayer = null;
            
            // Switch out of camera
            interactorCamera.Priority = -1;
            
            // Enable player input
            //GM.Player.InputSource.ActivateInput();
            
            // Hide buttons
            StopAllCoroutines();
            StartCoroutine(HideButtons());
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            Debug.Log("Player inside Nexus Teleporter");
        }
    }

    IEnumerator ShowButtons() {
        int index = 0;
        float t = 0f;
        while (t < showDelayBetweenButtons && index < uiButtonInstances.Count) {
            t += Time.deltaTime;
            if (t >= showDelayBetweenButtons) {
                uiButtonInstances[index].ShowButton();
                t = 0f;
                index++;
            }

            yield return null;
        }
    }

    IEnumerator HideButtons() {
        int index = uiButtonInstances.Count - 1;
        float t = 0f;
        while (t < hideDelayBetweenButtons && index > -1) {
            t += Time.deltaTime;
            if (t >= hideDelayBetweenButtons) {
                uiButtonInstances[index].HideButton();
                t = 0f;
                index--;
            }

            yield return null;
        }
    }
    
}
