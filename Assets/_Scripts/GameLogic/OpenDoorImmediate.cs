using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorImmediate : MonoBehaviour {

    [SerializeField] private DoorToggle door;

    void Awake() {
        door.ToggleImmediate(true);
    }
}