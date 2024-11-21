using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WaterInteractor : MonoBehaviour {
    [SerializeField] private VisualEffect splash;
    [SerializeField] private VisualEffect ripple;

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponentInChildren<Pushable>() != null) {
            splash.transform.position = other.transform.position;
            splash.Reinit();
            ripple.transform.position = other.transform.position;
            ripple.Reinit();
        }
    }
}
