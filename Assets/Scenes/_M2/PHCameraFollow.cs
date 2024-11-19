using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PHCameraFollow : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public void AssignPlayer(Player player) {
        virtualCamera.Follow = player.transform;
    }
}
