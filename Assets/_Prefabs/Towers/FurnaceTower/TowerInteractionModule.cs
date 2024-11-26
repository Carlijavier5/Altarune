using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class TowerInteractionModule : MonoBehaviour {

    private const string MOTION_TIME = "MotionTime",
                         BUTTON_STATE_SUFFIX = "-Button-Intro";

    [SerializeField] private Canvas canvas;
    [SerializeField] private Animator animator;
    [SerializeField] private Button[] inputButtons;
    [SerializeField] private float introTime;

    private int lerpParam, inputSize;
    private float timer;

    void Awake() {
        Camera.main.TryGetComponent(out CinemachineBrain cameraBrain);
        canvas.worldCamera = cameraBrain ? cameraBrain.OutputCamera
                                         : Camera.main;
        lerpParam = Animator.StringToHash(MOTION_TIME);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            Toggle(true);
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            Toggle(false);
        }
        if (Input.GetKeyDown(KeyCode.B)) {
            SetState(new TowerButtonData[] { new() });
        }
        if (Input.GetKeyDown(KeyCode.N)) {
            SetState(new TowerButtonData[] { new(), new() });
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            SetState(new TowerButtonData[] { new(), new(), new() });
        }
    }

    public void SetState(TowerButtonData[] inputActions) {
        inputSize = inputActions.Length;
        for (int i = 0; i < inputActions.Length; i++) {
            inputButtons[i].gameObject.SetActive(true);
            Button inputButton = inputButtons[i];
            TowerButtonData tbd = inputActions[i];

            inputButton.onClick.RemoveAllListeners();
            inputButton.onClick.AddListener(new(tbd.action));
            inputButton.image.sprite = tbd.sprite;
        }
        for (int i = inputActions.Length; i < inputButtons.Length; i++) {
            inputButtons[i].gameObject.SetActive(false);
        }
        Toggle(false);
    }

    private IEnumerator IAnimateCanvas(bool on) {
        float target = on ? introTime : 0;
        float lerpVal;
        while (timer != target) {
            timer = Mathf.MoveTowards(timer, target, Time.deltaTime);
            lerpVal = Mathf.Min(0.99f, timer / introTime);
            animator.SetFloat(lerpParam, lerpVal);
            yield return null;
        }

        animator.Play(inputSize + BUTTON_STATE_SUFFIX);
    }

    public void DebugSmth() {
        Debug.Log("I was clicked;");
    }

    public void Toggle(bool on) {
        if (inputSize > 0) {
            StopAllCoroutines();
            StartCoroutine(IAnimateCanvas(on));
        }
    }
}

public class TowerButtonData {
    public System.Action action;
    public Sprite sprite;

    public TowerButtonData() { }

    public TowerButtonData(System.Action action, Sprite sprite) {
        this.action = action;
        this.sprite = sprite;
    }
}