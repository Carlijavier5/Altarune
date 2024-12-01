using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomManager : MonoBehaviour {

    private static RoomManager instance;
    public static RoomManager Instance => instance;

    [SerializeField] private Player player;
    [SerializeField] private RoomControl[] rooms;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip track1, track2;

    private RoomControl currentRoom;
    
    //Event Delegates
    public delegate void RoomTransition();
    public event RoomTransition RoomStateTransition;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else instance = this;

        foreach (RoomControl room in rooms) {
            room.Init(player);
        } MoveToRoom(1, true);
    }

    void Update() {
        if (Input.GetKey(KeyCode.LeftControl)
            && Input.GetKeyDown(KeyCode.R)) {
            ForceCompleteRoom();
        }
    }

    private IEnumerator IChangeRoom(int roomIndex, bool immediateFade = false) {

        if (roomIndex <= 2) {
            if (musicSource.clip != track1) {
                musicSource.Stop();
                musicSource.clip = track1;
                musicSource.Play();
            }
        } else {
            if (musicSource.clip != track2) {
                musicSource.Stop();
                musicSource.clip = track2;
                musicSource.Play();
            }
        }

        roomIndex--;

        /// Fade Out
        if (RoomTransitionLoader.Instance != null) {
            RoomTransitionLoader.Instance.FadeOut(immediateFade);
        } 
        GM.TimeScaleManager.AddTimeScaleShift(0, 1);
        yield return new WaitForSecondsRealtime(1);

        /// Activate Current Room / Deactivate All Others
        for (int i = 0; i < rooms.Length; i++) {
            rooms[i].gameObject.SetActive(i == roomIndex);
        }

        /// Activate Current Room Camera / Deactivate Old Camera
        if (currentRoom) currentRoom.VirtualCamera.Priority = 0;
        RoomControl room = rooms[roomIndex];
        currentRoom = room;
        room.VirtualCamera.Priority = 10;

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

    public void MoveToRoom(int roomIndex, bool immediateFade = false) {
        StartCoroutine(IChangeRoom(roomIndex, immediateFade));
    }

    private void ForceCompleteRoom() {
        currentRoom.ForceCompletion();
    }
}