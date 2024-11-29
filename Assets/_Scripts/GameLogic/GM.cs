using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour {

    private static GM instance;
    public static GM Instance => instance;

    [SerializeField] private AudioManager audioManager;
    public static AudioManager AudioManager => instance.audioManager;

    [SerializeField] private DialogueManager dialogueManager;
    public static DialogueManager DialogueManager => instance.dialogueManager;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else instance = this;
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