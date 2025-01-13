using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderCondition : CCondition {
    private Collider collider;

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player _)) {
            Debug.Log("something?");
            CheckCondition();
        }
    }
}
