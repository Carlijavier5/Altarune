using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SniperTrackingLaser : MonoBehaviour 
{
    [SerializeField] Transform startTransform;
    [SerializeField] Transform endTransform;
    [SerializeField] Vector3 laserScale = Vector3.one;
    [SerializeField] bool debugMode;

    Vector3 startLocation;
    Vector3 endLocation;

    void Start() {
    }

    void Update() {
        if (debugMode) {
            endLocation = endTransform ? endTransform.position : transform.position;
        }

        UpdateLaserVisuals();
    }
    
    void UpdateLaserVisuals() {
        startLocation = startTransform.position;
        
        // Position
        transform.position = (startLocation + endLocation) * 0.5f;
        // Rotation
        transform.rotation = Quaternion.LookRotation((startLocation - endLocation).normalized);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + 90f, transform.eulerAngles.y, transform.eulerAngles.z);
        // Scale
        float newScale = (startLocation - endLocation).magnitude;
        transform.localScale = new Vector3(laserScale.x, newScale * 0.5f, laserScale.z);

    }
    
    public void SetTargetLocation(Vector3 targetLocation) {
        endLocation = targetLocation;
        // Redraw
        UpdateLaserVisuals();
    }
}
