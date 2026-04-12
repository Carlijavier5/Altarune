using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour {

    [SerializeField] private CinemachineCamera virtualCamera;

    public void AssignPlayer(PlayerController player) {
        virtualCamera.Follow = player.CameraTarget;
    }
}