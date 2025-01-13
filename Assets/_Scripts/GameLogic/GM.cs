using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour {

    public event System.Action OnPlayerInit;

    private static GM instance;
    public static GM Instance => instance;

    [SerializeField] private TransitionManager transitionManager;
    public static TransitionManager TransitionManager => instance.transitionManager;

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

    [SerializeField] private RoomManager roomManager;
    public static RoomManager RoomManager => instance.roomManager;

    [SerializeField] private LevelStateManager levelStateManager;
    public static LevelStateManager LeveStateManager => instance.levelStateManager;

    [SerializeField] private RunManager runManager;
    public static RunManager RunManager => instance.runManager;

    private Player player;
    public static Player Player {
        get => instance.player;
        set {
            if (instance.player == null
                    && value) {
                instance.player = value;
                instance.OnPlayerInit?.Invoke();
                instance.OnPlayerInit = null;
            }
        }
    }

    void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update() {
        if (Input.GetKey(KeyCode.LeftControl)
                && Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void DoGameOver() {
        RoomManager.MoveToRoom(RoomTag.Lab);
    }
}