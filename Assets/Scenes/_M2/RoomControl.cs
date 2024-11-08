using UnityEngine;
using Cinemachine;

public class RoomControl : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public CinemachineVirtualCamera VirtualCamera => virtualCamera;

    [SerializeField] private PHCameraFollow followTarget;

    [SerializeField] private Transform spawnPoint;
    public Transform SpawnPoint => spawnPoint;

    [SerializeField] private RoomCompletionListener listener;
    [SerializeField] private DoorToggle[] doors;

    void Awake() {
        listener.OnRoomCleared += Listener_OnRoomCleared;
    }

    private void Listener_OnRoomCleared() {
        foreach (DoorToggle door in doors) {
            door.ToggleDoorState();
        }
    }

    public void Init(Player player) {
        if (followTarget) followTarget.AssignPlayer(player);
    }

    public void ForceCompletion() => listener.CompleteRoom();
}
