using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExit : MonoBehaviour {

    [SerializeField] private int targetRoom;

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            RoomManager.Instance.MoveToRoom(targetRoom);
        }
    }
}