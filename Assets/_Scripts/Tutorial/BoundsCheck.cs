using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCheck : MonoBehaviour {
    public TutorialManager manager;
    public void OnTriggerEnter(Collider other) {
        manager.BatteryTrack();
        Debug.Log("triggered");
    }
}
