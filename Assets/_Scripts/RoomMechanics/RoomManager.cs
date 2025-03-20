using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour {

    [SerializeField] private RoomIdentifier[] roomIdentifiers;
    public RoomIdentifier[] RoomIdentifiers => roomIdentifiers;

    private RoomControl currentRoom;
    public RoomControl CurrentRoom {
        get => currentRoom;
        set {
            if (!currentRoom && value) {
                currentRoom = value;
                sourceRoomTag = currentRoomTag;
            } else Debug.LogWarning("Invalid Room Control Assignment");
        }
    }

    private readonly Dictionary<RoomTag, int> roomIndexMap = new();
    private RoomTag sourceRoomTag, currentRoomTag;
    public RoomTag SourceRoomTag => sourceRoomTag;

    public bool usedOnlyLight = true;
    public bool usedSniperRailgunTowerOnly = true;

    void Awake() {
        foreach (RoomIdentifier roomID in roomIdentifiers) {
            int buildIndex = roomID.roomScene.BuildIndex;
            roomIndexMap[roomID.roomTag] = buildIndex;
        }
    }

    void Update() {
        if (Input.GetKey(KeyCode.LeftControl)
                && Input.GetKeyDown(KeyCode.R)) {
            ForceCompleteRoom();
        }
        if (Input.GetKey(KeyCode.LeftControl)
                && Input.GetKeyDown(KeyCode.L)) {
            Transform spawnPoint = currentRoom.SpawnPoint;
            GM.Player.TryTeleport(spawnPoint.position);
        }
        if (Input.GetKey(KeyCode.LeftControl)
            && Input.GetKeyDown(KeyCode.Alpha4)) {
            MoveToRoom(RoomTag.F4);
        }
        if (Input.GetKey(KeyCode.LeftControl)
                && Input.GetKeyDown(KeyCode.Alpha8)) {
            MoveToRoom(RoomTag.F8);
        }
        if (Input.GetKeyDown(KeyCode.End)) {
            GM.Instance.FinishGame();
        }
    }

    public void MoveToRoom(RoomTag roomTag) {
        usedSniperRailgunTowerOnly = true;
        if ((int) roomTag > 1 && usedOnlyLight) {
            if (AchievementManager.Instance) {
                AchievementManager.Instance.ChainLightningOnlyCheck();
            }
        }
        usedOnlyLight = true;
        currentRoomTag = roomTag;
        GM.TransitionManager.FadeOut();
        GM.TimeScaleManager.AddTimeScaleShift(0, 1);
        GM.TransitionManager.OnFadeEnd += TransitionManager_OnFadeEnd;
    }

    private void TransitionManager_OnFadeEnd() {
        GM.TransitionManager.OnFadeEnd -= TransitionManager_OnFadeEnd;
        int targetIndex = roomIndexMap[currentRoomTag];
        SceneManager.LoadScene(targetIndex, LoadSceneMode.Single);
    }

    public void FinalizeRoomTransition() {
        GM.TimeScaleManager.AddTimeScaleShift(0, 1);
        GM.TransitionManager.FadeIn();
    }

    private void ForceCompleteRoom() {
        currentRoom.ForceCompletion();
    }
}

[System.Serializable]
public class RoomIdentifier {
    public RoomTag roomTag;
    public SceneRef roomScene;
}