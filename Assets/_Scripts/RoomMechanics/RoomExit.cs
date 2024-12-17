using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExit : MonoBehaviour {

    [SerializeField] private RoomTag targetRoom;

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player _)) {
            GM.RoomManager.MoveToRoom(targetRoom);
        }
    }
}