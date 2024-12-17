using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour {

    [SerializeField] private RoomManager roomManager;

    private int runHealth;
    private float runMana;

    private readonly Dictionary<RoomTag, RoomStatus> roomStatusMap = new();

    void Awake() {
        RoomIdentifier[] roomIDs = roomManager.RoomIdentifiers;
        foreach (RoomIdentifier roomID in roomIDs) {
            roomStatusMap[roomID.roomTag] = new();
        }
    }

    public bool IsVisited(RoomTag roomTag) {
        return roomStatusMap.TryGetValue(roomTag, out RoomStatus status)
               && status.IsVisited;
    }

    public bool IsComplete(RoomTag roomTag) {
        return roomStatusMap.TryGetValue(roomTag, out RoomStatus status)
               && status.isComplete;
    }

    public HashSet<RoomTag> GetClearedExits(RoomTag roomTag) {
        HashSet<RoomTag> exits = roomStatusMap.TryGetValue(roomTag, out RoomStatus status)
                               ? new(status.exitsCleared) : null;
        return exits;
    }

    public Dictionary<string, PerishInfo> GetPerishMap(RoomTag roomTag) {
        Dictionary<string, PerishInfo> perishMap = roomStatusMap.TryGetValue(roomTag, out RoomStatus status)
                                                 ? new(status.perishMap) : null;
        return perishMap;
    }

    public void AddClearedExit(RoomTag roomTag, RoomTag exitTag) {
        if (roomStatusMap.TryGetValue(roomTag, out RoomStatus status)) {
            status.exitsCleared.Add(exitTag);
        }
    }

    public void AddPerishInfo(RoomTag roomTag, PerishInfo info) {
        if (roomStatusMap.TryGetValue(roomTag, out RoomStatus status)) {
            status.perishMap[info.name] = info;
        }
    }

    public void CompleteRoom(RoomTag roomTag) {
        if (roomStatusMap.TryGetValue(roomTag, out RoomStatus status)) {
            status.isComplete = true;
        }
    }
}

public class RoomStatus {
    public HashSet<RoomTag> exitsCleared = new();
    public bool IsVisited => exitsCleared.Count > 0;
    public bool isComplete;

    public Dictionary<string, PerishInfo> perishMap = new();
}

public class PerishInfo {
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}