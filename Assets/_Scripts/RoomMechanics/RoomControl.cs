using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;

public enum RoomTag { Lab, F1, F2, F3, C1, F4, C2, F5, F6, F7, C3, F8 }

public class RoomControl : MonoBehaviour {

    [SerializeField] private RoomTag roomTag;

    [SerializeField] private Volume volume;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public CinemachineVirtualCamera VirtualCamera => virtualCamera;

    [SerializeField] private CameraFollow followTarget;

    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private Transform defaultSpawnPoint;
    public Transform SpawnPoint => defaultSpawnPoint;

    [SerializeField] private RoomCompletionListener listener;
    [SerializeField] private DoorToggle[] doors;

    private readonly Dictionary<RoomTag, Transform> spawnPointMap = new();

    void Awake() {
        listener.OnRoomCleared += Listener_OnRoomCleared;
        foreach (SpawnPoint spawnPoint in spawnPoints) {
            spawnPointMap[spawnPoint.originTag] = spawnPoint.spawnTransform;
        }
    }

    public void SwitchVolume() {
        if (volume) {
            volume.sharedProfile = volume.profile;
        }
    }

    public Transform GetSpawnPoint(RoomTag roomTag) {
        if (spawnPointMap.TryGetValue(roomTag, out Transform spawnPoint)) {
            return spawnPoint;
        } else return defaultSpawnPoint;
    }

    private void Listener_OnRoomCleared() {
        foreach (DoorToggle door in doors) {
            door.ToggleDoorState();
        }
    }

    public void Init(Player player) {
        if (followTarget) {
            followTarget.AssignPlayer(player.InputSource);
        }
    }

    public void ForceCompletion() => listener.CompleteRoom();
}

[System.Serializable]
public class SpawnPoint {
    public RoomTag originTag;
    public Transform spawnTransform;
} 