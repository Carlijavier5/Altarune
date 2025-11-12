using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CanvasCameraUpdater : MonoBehaviour {
    [SerializeField] private Canvas canvas;
    private Camera uiCamera;

    void Awake() {
        UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
        if (cameraData.cameraStack.Count == 1) {
            uiCamera = cameraData.cameraStack[0];
        }
        canvas.worldCamera = uiCamera;
    }
}