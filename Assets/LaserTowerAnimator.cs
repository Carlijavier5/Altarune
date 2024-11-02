using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LaserTowerAnimator : MonoBehaviour {
    [SerializeField] private GameObject laserFX;

    private void Start() {
        laserFX.GetComponentInChildren<VisualEffect>().Stop();
    }

    public void PlayLaser(Quaternion rotation) {
        laserFX.transform.rotation = rotation;
        laserFX.GetComponentInChildren<VisualEffect>().Play();
    }
}
