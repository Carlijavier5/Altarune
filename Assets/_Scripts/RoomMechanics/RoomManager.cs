using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    private static RoomManager instance;
    public static RoomManager Instance => instance;

    [SerializeField] private Player player;
    [SerializeField] private RoomIdentifier[] roomIdentifiers;

    private RoomControl currentRoom;
    
    //Event Delegates
    public delegate void RoomTransition();
    public event RoomTransition RoomStateTransition;

    private readonly Dictionary<RoomTag, RoomControl> roomMap = new();
    private RoomTag currRoomTag;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else instance = this;

        foreach (RoomIdentifier roomID in roomIdentifiers) {
            RoomControl roomControl = roomID.roomControl;
            roomMap[roomID.roomTag] = roomControl;
            roomControl.Init(player);
        }

        MoveToRoom(RoomTag.F1, true);
    }

    void Update() {
        if (Input.GetKey(KeyCode.LeftControl)
            && Input.GetKeyDown(KeyCode.R)) {
            ForceCompleteRoom();
        }
        if (Input.GetKey(KeyCode.LeftControl)
            && Input.GetKeyDown(KeyCode.L)) {
            RoomControl room = roomMap[currRoomTag];
            Transform spawnPoint = room.SpawnPoint;
            player.TryTeleport(spawnPoint.position);
        }
    }

    private IEnumerator IChangeRoom(RoomTag roomTag, bool immediateFade = false) {

        currRoomTag = roomTag;

        /// Fade Out
        if (RoomTransitionLoader.Instance != null) {
            RoomTransitionLoader.Instance.FadeOut(immediateFade);
        } 
        GM.TimeScaleManager.AddTimeScaleShift(0, 1);
        yield return new WaitForSecondsRealtime(1);

        /// Activate Current Room / Deactivate All Others
        foreach (KeyValuePair<RoomTag, RoomControl> kvp in roomMap) {
            RoomTag tag = kvp.Key;
            RoomControl control = kvp.Value;
            control.gameObject.SetActive(tag == roomTag);
        }

        /// Activate Current Room Camera / Deactivate Old Camera
        if (currentRoom) currentRoom.VirtualCamera.Priority = 0;
        RoomControl room = roomMap[roomTag];
        currentRoom = room;
        room.VirtualCamera.Priority = 10;
        yield return new WaitForSecondsRealtime(0.5f);

        /// Teleport Player to Current Room
        Transform spawnPoint = room.SpawnPoint;
        player.TryTeleport(spawnPoint.position);

        yield return new WaitForSecondsRealtime(1);

        /// Fade In
        GM.TimeScaleManager.AddTimeScaleShift(0, 1);
        if (RoomTransitionLoader.Instance != null) {
            RoomTransitionLoader.Instance.FadeIn();
        }
    }

    public void MoveToRoom(RoomTag roomTag, bool immediateFade = false) {
        StartCoroutine(IChangeRoom(roomTag, immediateFade));
    }

    private void ForceCompleteRoom() {
        currentRoom.ForceCompletion();
    }
}

[System.Serializable]
public class RoomIdentifier {
    public RoomTag roomTag;
    public RoomControl roomControl;
}