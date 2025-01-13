using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    [SerializeField] private List<DialogueData> intro;
    // Start is called before the first frame update

    private IEnumerator _activeTimer;
    private bool pausedDialogue;

    private int dialogueIndex = 0;

    private bool _spaceCompletion = false;
    private bool _batteryCompletion;
    private bool _inBattery;
    private int batteryIndex = 0;
    private bool _towerCompletion;

    [SerializeField] private Transform checks;

    private PlayerController player;
    void Start() {
        StartCoroutine(RunIntroSequence());
        player = FindObjectOfType<PlayerController>();
        GM.DialogueManager.OnDialogueEnd += RunDialogueSequence;
        checks.DOMoveY(300f, 0f);
    }

    private void RunDialogueSequence() {
        switch (dialogueIndex) {
            case 0:
                player.enabled = true;
                break;
            case 1:
                player.enabled = true;
                checks.DOMoveY(-300f, 1f);
                checks.GetChild(0).gameObject.SetActive(true);
                break;
        }
        dialogueIndex++;
    }

    private IEnumerator RunSpaceCheck() {
        GM.DialogueManager.DoDialogue(intro[1]);
        yield return null;
    }
    
    private IEnumerator RunQCheck() {
        pausedDialogue = true;
        GM.DialogueManager.DoDialogue(intro[2]);
        yield return null;
    }

    private IEnumerator RunIntroSequence() {
        yield return new WaitForSeconds(1f);
        GM.DialogueManager.DoDialogue(intro[0]);
        player.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_activeTimer == null && pausedDialogue) {
            _activeTimer = Timer();
            StartCoroutine(_activeTimer);
        }

        if (_inBattery) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                batteryIndex++;
                Debug.Log(batteryIndex);
            }
            if (batteryIndex > 4) StartCoroutine(RunQCheck());
        }
    }

    private IEnumerator Timer() {
        while (pausedDialogue) {
            yield return new WaitForSecondsRealtime(1f);//bruh
            GM.TimeScaleManager.AddTimeScaleShift(0f, 1f);
            _activeTimer = null;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("triggered");
        if (!_spaceCompletion) {
            _spaceCompletion = true;
            StartCoroutine(RunSpaceCheck());
            player.enabled = false;
        }
    }

    public void BatteryTrack() {
        if (!_spaceCompletion) {
            _spaceCompletion = true;
            StartCoroutine(RunSpaceCheck());
            player.enabled = false;
        }
    }
}
