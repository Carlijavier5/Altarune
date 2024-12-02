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

    [SerializeField] private TimeScaleManager timeScaleManager;
    public static TimeScaleManager TimeScaleManager => instance.timeScaleManager;

    [SerializeField] private ScreenEffectManager screenEffectManager;
    public static ScreenEffectManager ScreenEffectManager => instance.screenEffectManager;

    [SerializeField] private CameraShakeManager cameraShakeManager;
    public static CameraShakeManager CameraShakeManager => instance.cameraShakeManager;

    [SerializeField] private InventoryManager inventoryManager;
    public static InventoryManager InventoryManager => instance.inventoryManager;

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