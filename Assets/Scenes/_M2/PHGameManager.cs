using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PHGameManager : MonoBehaviour {

    private static PHGameManager instance;
    public static PHGameManager Instance => instance;

    [SerializeField] private CinemachineBrain cameraBrain;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Player player;
    [SerializeField] private RoomControl[] rooms;

    private RoomControl currentRoom;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else instance = this;

        playerController.Init(cameraBrain);
        foreach (RoomControl room in rooms) room.Init(player);
        ChangeRoom(1);
    }

    void Update() {
        if (Input.GetKey(KeyCode.LeftControl)
            && Input.GetKeyDown(KeyCode.R)) {
            ForceCompleteRoom();
        }
    }

    private IEnumerator IChangeRoom(int roomIndex) {
        roomIndex--;

        RoomTransitionLoader.Instance.FadeOut();
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;

        for (int i = 0; i < rooms.Length; i++) {
            rooms[i].gameObject.SetActive(i == roomIndex);
        }

        if (currentRoom) currentRoom.VirtualCamera.Priority = 0;

        RoomControl room = rooms[roomIndex];
        currentRoom = room;

        room.VirtualCamera.Priority = 10;

        Transform spawnPoint = room.SpawnPoint;
        player.TryTeleport(spawnPoint.position);

        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 0;
        RoomTransitionLoader.Instance.FadeIn();
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
    }

    public void ChangeRoom(int roomIndex) {
        StartCoroutine(IChangeRoom(roomIndex));
    }

    public void ForceCompleteRoom() {
        currentRoom.ForceCompletion();
    }
}