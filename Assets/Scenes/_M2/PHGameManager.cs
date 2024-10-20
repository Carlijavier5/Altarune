using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PHGameManager : MonoBehaviour {

    private static PHGameManager instance;
    public static PHGameManager Instance => instance;

    [SerializeField] private CinemachineBrain cameraBrain;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Player player;
    [SerializeField] private RoomControl[] rooms;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip track1, track2;

    private RoomControl currentRoom;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else instance = this;

        playerController.Init(cameraBrain);
        foreach (RoomControl room in rooms) room.Init(player);
        ChangeRoom(1, true);
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

        if (RoomTransitionLoader.Instance != null) RoomTransitionLoader.Instance.FadeOut(immediateFade);
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
        if (RoomTransitionLoader.Instance != null) RoomTransitionLoader.Instance.FadeIn();
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
    }

    public void ChangeRoom(int roomIndex, bool immediateFade = false) {
        StartCoroutine(IChangeRoom(roomIndex, immediateFade));
    }

    public void ForceCompleteRoom() {
        currentRoom.ForceCompletion();
    }

    public void DoGameOver() {
        StartCoroutine(RestartScene());
    }

    private IEnumerator RestartScene() {
        RoomTransitionLoader.Instance.FadeOut();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}