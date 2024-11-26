using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarReflection : MonoBehaviour {
    [SerializeField] private Camera reflectCamera;
    [SerializeField] private RenderTexture reflectTex;
    [SerializeField] private bool nonPlanar = false;
    [SerializeField] private Transform mirrorPos;

    private void LateUpdate() {
        reflectCamera.fieldOfView = Camera.main.fieldOfView;
        Vector3 camPos = Camera.main.transform.position;
        Quaternion camRot = Camera.main.transform.rotation;
        if (!nonPlanar) {
            reflectCamera.transform.position = new Vector3(camPos.x, -camPos.y + transform.position.y, camPos.z);
            reflectCamera.transform.rotation = Quaternion.Euler(-camRot.eulerAngles.x, camRot.eulerAngles.y, 0f);
        }
        else {
            reflectCamera.transform.position = mirrorPos.position;
            reflectCamera.transform.rotation = Quaternion.Euler(-camRot.eulerAngles.x + 60, camRot.eulerAngles.y + 180, 0f);
        }
    }
}
