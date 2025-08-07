using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraUpdater : MonoBehaviour {
    [SerializeField] private Canvas canvas;

    void Update() {
        canvas.worldCamera = Camera.main;
    }
}
