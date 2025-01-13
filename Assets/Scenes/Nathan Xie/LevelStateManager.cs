using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStateManager : MonoBehaviour {

    private readonly HashSet<RoomTag> roomStateSet = new();

    public bool IsComplete(RoomTag roomTag) {
        return roomStateSet.Contains(roomTag);
    }

    public void CompleteRoom(RoomTag roomTag) {
        roomStateSet.Add(roomTag);
    }
}