using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public void AssignPlayer(PlayerController player) {
        virtualCamera.Follow = player.CameraTarget;
    }
}