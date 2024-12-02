using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTP : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            player.TryTeleport(player.LastGroundPoint + Vector3.up * 0.5f);
        }
    }
}