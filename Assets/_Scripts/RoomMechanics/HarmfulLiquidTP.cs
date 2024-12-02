using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarmfulLiquidTP : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            player.TryDamage(1);
            player.TryTeleport(player.LastGroundPoint + Vector3.up * 0.5f);
        }
    }
}