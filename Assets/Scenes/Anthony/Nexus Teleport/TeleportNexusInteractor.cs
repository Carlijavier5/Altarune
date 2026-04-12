using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class TeleportNexusInteractor : MonoBehaviour {
    
    [Header("Interaction")]
    [SerializeField] private BoxCollider collider;
    [SerializeField] private CinemachineCamera interactorCamera;

    private Canvas canvas;
    private Player interactingPlayer;

    [Space] [Header("Teleport")]
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private TeleportNexusUIButton uiButtonPrefab;
    [SerializeField] private List<RoomTag> roomTagEntries = new();

    List<TeleportNexusUIButton> uiButtonInstances;

    [Space] [Header("Display Settings")]
    [SerializeField] private float showDelayBetweenButtons = 0.2f;
    [SerializeField] private float hideDelayBetweenButtons = 0.1f;
    
    void Start() {
        if (!uiButtonPrefab) {
            Debug.LogError("[Teleport Nexus Interactor] uiButtonPrefab is unset or missing!!");
            return;
        }

        uiButtonInstances = new List<TeleportNexusUIButton>();
        List<TeleportNexusUIButton> buttonInstancesFound = GetComponentsInChildren<TeleportNexusUIButton>().ToList();
        // Filter unlocked rooms
        for (int i = 0; i < buttonInstancesFound.Count; i++) {
            if (!GM.LeveStateManager.HasRoom(buttonInstancesFound[i].RoomTag)) {
                continue;
            }
            uiButtonInstances.Add(buttonInstancesFound[i]);
        }

        canvas = GetComponentInChildren<Canvas>();
        if (canvas) {
            canvas.worldCamera = Camera.main;
        }
    }

    void Update()
    {
    }

    bool CanPlayerInteract() => uiButtonInstances.Count > 0;

    void OnTriggerEnter(Collider other) {
        if (!CanPlayerInteract()) {
            return;
        }
        
        if (other.TryGetComponent(out Player player)) {
            interactingPlayer = player;
            
            // Switch to camera
            interactorCamera.Priority = 99;
            
            // Show buttons
            StopAllCoroutines();
            StartCoroutine(ShowButtons());
        }
    }

    void OnTriggerExit(Collider other) {
        if (!CanPlayerInteract()) {
            return;
        }
        
        if (other.TryGetComponent(out Player player)) {
            interactingPlayer = null;
            
            // Switch out of camera
            interactorCamera.Priority = -1;
            
            // Hide buttons
            StopAllCoroutines();
            StartCoroutine(HideButtons());
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
