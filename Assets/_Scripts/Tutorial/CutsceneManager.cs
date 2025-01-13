using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour {
    [SerializeField] private List<CCondition> conditions;
    private PlayerController player;
    private void Awake() {
        RegisterEvents();
    }

    private void RegisterEvents() {
        foreach (CCondition condition in conditions) {
            condition.OnConditionTrigger += RunEvent;
        }
    }

    private void RunEvent(CConditionData data) {
        GM.DialogueManager.OnDialogueEnd += EndOfEventAction;
        if (data.dialogueData) GM.DialogueManager.DoDialogue(data.dialogueData);
        if (GM.Player) {
            GM.Player.InputSource.DeactivateInput();
            Debug.Log("help");
        }
        else {
            Debug.Log("player is null");
        }
    }

    private void EndOfEventAction() {
        GM.DialogueManager.OnDialogueEnd -= EndOfEventAction;
        GM.Player.InputSource.ActivateInput();
    }
}
