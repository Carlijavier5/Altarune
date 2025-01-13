using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStateManager : MonoBehaviour {

    private readonly HashSet<RoomTag> roomStateSet = new();

    public bool HasRoom(RoomTag roomTag) {
        return roomStateSet.Contains(roomTag);
    }

    public void AddRoom(RoomTag roomTag) {
        roomStateSet.Add(roomTag);
    }
}